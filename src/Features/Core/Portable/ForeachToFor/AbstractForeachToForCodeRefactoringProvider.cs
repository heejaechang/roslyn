﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.Editing;
using Microsoft.CodeAnalysis.Formatting;
using Microsoft.CodeAnalysis.LanguageServices;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.CodeAnalysis.Shared.Extensions;
using Microsoft.CodeAnalysis.Shared.Utilities;
using Microsoft.CodeAnalysis.Simplification;
using Microsoft.CodeAnalysis.Text;
using Roslyn.Utilities;

namespace Microsoft.CodeAnalysis.ForeachToFor
{
    internal abstract class AbstractForEachToForCodeRefactoringProvider<TForEachStatement> :
        CodeRefactoringProvider
            where TForEachStatement : SyntaxNode
    {
        private const string get_Count = nameof(get_Count);
        private const string get_Item = nameof(get_Item);

        private const string Length = nameof(Array.Length);
        private const string Count = nameof(IList.Count);

        private static readonly ImmutableArray<string> s_KnownInterfaceNames =
            new string[] { typeof(IList<>).FullName, typeof(IReadOnlyList<>).FullName, typeof(IList).FullName }.ToImmutableArray();

        protected abstract TForEachStatement GetForEachStatement(TextSpan selelction, SyntaxToken token);
        protected abstract bool ValidLocation(ForEachInfo foreachInfo);
        protected abstract (SyntaxNode start, SyntaxNode end) GetForEachBody(TForEachStatement foreachStatement);
        protected abstract void ConvertToForStatement(SemanticModel model, ForEachInfo info, SyntaxEditor editor, CancellationToken cancellationToken);

        public override async Task ComputeRefactoringsAsync(CodeRefactoringContext context)
        {
            var document = context.Document;
            var cancellationToken = context.CancellationToken;

            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            var token = root.FindToken(context.Span.Start);

            var foreachStatement = GetForEachStatement(context.Span, token);
            if (foreachStatement == null)
            {
                return;
            }

            var model = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);

            var semanticFact = document.GetLanguageService<ISemanticFactsService>();
            var foreachInfo = GetForeachInfo(semanticFact, model, foreachStatement, cancellationToken);
            if (foreachInfo == null)
            {
                return;
            }

            if (!ValidLocation(foreachInfo))
            {
                return;
            }

            context.RegisterRefactoring(
                new ForEachToForCodeAction(
                    FeaturesResources.Convert_foreach_to_for,
                    c => ConvertForeachToForAsync(document, foreachInfo, c)));
        }

        protected string CreateUniqueName(SemanticModel model, SyntaxNode contextNode, string baseName)
        {
            Contract.ThrowIfNull(contextNode);
            Contract.ThrowIfNull(baseName);

            return NameGenerator.GenerateUniqueName(baseName, string.Empty,
               name => model.LookupSymbols(contextNode.SpanStart, /*container*/null, name).Length == 0);
        }

        protected SyntaxNode GetCollectionVariableName(
            SemanticModel model, SyntaxGenerator generator, ForEachInfo foreachInfo, SyntaxNode foreachCollectionExpression)
        {
            if (foreachInfo.RequireCollectionStatement)
            {
                return generator.IdentifierName(
                    CreateUniqueName(model, foreachInfo.ForEachStatement, foreachInfo.CollectionNameSuggestion));
            }

            return foreachCollectionExpression.WithoutTrivia().WithAdditionalAnnotations(Formatter.Annotation);
        }

        protected void IntroduceCollectionStatement(
            SemanticModel model, ForEachInfo foreachInfo, SyntaxEditor editor, SyntaxNode foreachCollectionExpression, SyntaxNode collectionVariable)
        {
            if (!foreachInfo.RequireCollectionStatement)
            {
                return;
            }

            // TODO: refactor introduce variable refactoring to real service and use that service here to introduce local variable
            var generator = editor.Generator;

            // 
            var collectionVariableToken = generator.Identifier(collectionVariable.ToString()).WithAdditionalAnnotations(RenameAnnotation.Create());

            // this expression is from user code. don't simplify this.
            var expression = foreachCollectionExpression.WithoutAnnotations(SimplificationHelpers.DontSimplifyAnnotation);
            var collectionStatement = generator.LocalDeclarationStatement(
                collectionVariableToken,
                foreachInfo.RequireExplicitCastInterface
                    ? generator.CastExpression(foreachInfo.ExplicitCastInterface, expression) : expression);

            // attach trivia to right place
            collectionStatement = collectionStatement.WithLeadingTrivia(foreachInfo.ForEachStatement.GetFirstToken().LeadingTrivia);

            editor.InsertBefore(foreachInfo.ForEachStatement, collectionStatement);
        }

        protected SyntaxNode AddItemVariableDeclaration(
            SyntaxGenerator generator, SyntaxNode type, string foreachVariableString,
            ITypeSymbol castType, SyntaxNode collectionVariableName, string indexString)
        {
            var memberAccess = generator.ElementAccessExpression(
                    collectionVariableName, generator.IdentifierName(indexString));

            return generator.LocalDeclarationStatement(
                type, foreachVariableString, generator.CastExpression(castType, memberAccess));
        }

        private ForEachInfo GetForeachInfo(ISemanticFactsService semanticFact, SemanticModel model, TForEachStatement foreachStatement, CancellationToken cancellationToken)
        {
            var operation = model.GetOperation(foreachStatement, cancellationToken) as IForEachLoopOperation;
            if (operation == null || operation.Locals.Length != 1)
            {
                return null;
            }

            var foreachVariable = operation.Locals[0];
            if (foreachVariable == null)
            {
                return null;
            }

            // VB can have Next variable. but we only support
            // simple 1 variable case.
            if (operation.NextVariables.Length > 1)
            {
                return null;
            }

            if (!operation.NextVariables.IsEmpty)
            {
                var nextVariable = operation.NextVariables[0] as ILocalReferenceOperation;
                if (nextVariable == null || nextVariable.Local?.Equals(foreachVariable) == false)
                {
                    // we do not support anything else than local reference for next variable
                    // operation
                    return null;
                }
            }

            if (CheckIfForEachVariableIsWrittenInside(model, foreachVariable, foreachStatement))
            {
                return null;
            }

            var foreachCollection = RemoveImplicitConversion(operation.Collection);
            if (foreachCollection == null)
            {
                return null;
            }

            GetInterfaceInfo(semanticFact, model, foreachVariable, foreachCollection,
                out var explicitCastInterface, out var collectionNameSuggestion, out var countName);
            if (countName == null)
            {
                return null;
            }

            var requireCollectionStatement = CheckRequireCollectionStatement(foreachCollection);
            return new ForEachInfo(collectionNameSuggestion, countName, explicitCastInterface, foreachVariable.Type, requireCollectionStatement, foreachStatement);
        }

        private static void GetInterfaceInfo(
            ISemanticFactsService semanticFact, SemanticModel model, ILocalSymbol foreachVariable, IOperation foreachCollection,
            out ITypeSymbol explicitCastInterface, out string collectionNameSuggestion, out string countName)
        {
            explicitCastInterface = default;
            collectionNameSuggestion = default;
            countName = default;

            // go through list of types and interfaces to find out right set;
            var foreachType = foreachVariable.Type;
            if (IsNullOrErrorType(foreachType))
            {
                return;
            }

            var collectionType = foreachCollection.Type;
            if (IsNullOrErrorType(collectionType))
            {
                return;
            }

            // go through explicit types first.

            // check array case
            if (collectionType is IArrayTypeSymbol array && array.Rank == 1)
            {
                if (!IsExchangable(semanticFact, array.ElementType, foreachType, model.Compilation))
                {
                    return;
                }

                collectionNameSuggestion = "array";
                explicitCastInterface = null;
                countName = Length;
                return;
            }

            // check string case
            if (collectionType.Equals(model.Compilation.GetSpecialType(SpecialType.System_String)))
            {
                var charType = model.Compilation.GetSpecialType(SpecialType.System_Char);
                if (!IsExchangable(semanticFact, charType, foreachType, model.Compilation))
                {
                    return;
                }

                collectionNameSuggestion = "str";
                explicitCastInterface = null;
                countName = Length;
                return;
            }

            // check ImmutableArray case 
            if (collectionType.OriginalDefinition.Equals(model.Compilation.GetTypeByMetadataName(typeof(ImmutableArray<>).FullName)))
            {
                var indexer = GetInterfaceMember(collectionType, get_Item);
                if (indexer != null)
                {
                    if (!IsExchangable(semanticFact, indexer.ReturnType, foreachType, model.Compilation))
                    {
                        return;
                    }

                    collectionNameSuggestion = "array";
                    explicitCastInterface = null;
                    countName = Length;
                    return;
                }
            }

            // go through all known interfaces we support next.
            var knownCollectionInterfaces = s_KnownInterfaceNames.Select(
                s => model.Compilation.GetTypeByMetadataName(s)).Where(t => !IsNullOrErrorType(t));

            // for all interfaces, we suggest collection name as "list"
            collectionNameSuggestion = "list";

            // check type itself is interface case
            if (collectionType.TypeKind == TypeKind.Interface && knownCollectionInterfaces.Contains(collectionType.OriginalDefinition))
            {
                var indexer = GetInterfaceMember(collectionType, get_Item);
                if (indexer != null &&
                    IsExchangable(semanticFact, indexer.ReturnType, foreachType, model.Compilation))
                {
                    explicitCastInterface = null;
                    countName = Count;
                    return;
                }
            }

            // check regular cases (implicitly implemented)
            ITypeSymbol explicitInterface = null;
            foreach (var current in collectionType.AllInterfaces)
            {
                if (!knownCollectionInterfaces.Contains(current.OriginalDefinition))
                {
                    continue;
                }

                // see how the type implements the interface
                var countSymbol = GetInterfaceMember(current, get_Count);
                var indexerSymbol = GetInterfaceMember(current, get_Item);
                if (countSymbol == null || indexerSymbol == null)
                {
                    continue;
                }

                var countImpl = collectionType.FindImplementationForInterfaceMember(countSymbol) as IMethodSymbol;
                var indexerImpl = collectionType.FindImplementationForInterfaceMember(indexerSymbol) as IMethodSymbol;
                if (countImpl == null || indexerImpl == null)
                {
                    continue;
                }

                if (!IsExchangable(semanticFact, indexerImpl.ReturnType, foreachType, model.Compilation))
                {
                    continue;
                }

                // implicitly implemented!
                if (countImpl.ExplicitInterfaceImplementations.IsEmpty &&
                    indexerImpl.ExplicitInterfaceImplementations.IsEmpty)
                {
                    explicitCastInterface = null;
                    countName = Count;
                    return;
                }

                if (explicitInterface == null)
                {
                    explicitInterface = current;
                }
            }

            // okay, we don't have implicitly implemented one, but we do have explicitly implemented one
            if (explicitInterface != null)
            {
                explicitCastInterface = explicitInterface;
                countName = Count;
            }
        }

        private static bool IsExchangable(
            ISemanticFactsService semanticFact, ITypeSymbol type1, ITypeSymbol type2, Compilation compilation)
        {
            return semanticFact.IsAssignableTo(type1, type2, compilation) ||
                   semanticFact.IsAssignableTo(type2, type1, compilation);
        }

        private static bool IsNullOrErrorType(ITypeSymbol type)
        {
            return type == null || type is IErrorTypeSymbol;
        }

        private static IMethodSymbol GetInterfaceMember(ITypeSymbol interfaceType, string memberName)
        {
            foreach (var current in interfaceType.GetAllInterfacesIncludingThis())
            {
                var members = current.GetMembers(memberName);
                if (!members.IsEmpty && members[0] is IMethodSymbol method)
                {
                    return method;
                }
            }

            return null;
        }

        private static bool CheckRequireCollectionStatement(IOperation operation)
        {
            return operation.Kind != OperationKind.LocalReference &&
                   operation.Kind != OperationKind.FieldReference &&
                   operation.Kind != OperationKind.ParameterReference &&
                   operation.Kind != OperationKind.PropertyReference &&
                   operation.Kind != OperationKind.ArrayElementReference;
        }

        private IOperation RemoveImplicitConversion(IOperation collection)
        {
            if (collection is IConversionOperation conversion && conversion.IsImplicit)
            {
                return RemoveImplicitConversion(conversion.Operand);
            }

            return collection;
        }

        private bool CheckIfForEachVariableIsWrittenInside(SemanticModel semanticModel, ISymbol foreachVariable, TForEachStatement foreachStatement)
        {
            var (start, end) = GetForEachBody(foreachStatement);
            if (start == null || end == null)
            {
                // empty body. this can happen in VB
                return false;
            }

            var dataFlow = semanticModel.AnalyzeDataFlow(start, end);

            if (!dataFlow.Succeeded)
            {
                // if we can't get good analysis, assume it is written
                return true;
            }

            return dataFlow.WrittenInside.Contains(foreachVariable);
        }

        private async Task<Document> ConvertForeachToForAsync(
            Document document,
            ForEachInfo foreachInfo,
            CancellationToken cancellationToken)
        {
            var model = await document.GetSemanticModelAsync(cancellationToken).ConfigureAwait(false);
            var workspace = document.Project.Solution.Workspace;
            var editor = new SyntaxEditor(model.SyntaxTree.GetRoot(cancellationToken), workspace);

            ConvertToForStatement(model, foreachInfo, editor, cancellationToken);

            var newRoot = editor.GetChangedRoot();
            return document.WithSyntaxRoot(newRoot);
        }

        protected class ForEachInfo
        {
            public ForEachInfo(
                string collectionNameSuggestion, string countName, ITypeSymbol explicitCastInterface, ITypeSymbol forEachElementType,
                bool requireCollectionStatement, TForEachStatement forEachStatement)
            {
                CollectionNameSuggestion = collectionNameSuggestion;
                CountName = countName;

                // order of setting properties is important here
                ExplicitCastInterface = explicitCastInterface;
                ForEachElementType = forEachElementType;

                RequireCollectionStatement = requireCollectionStatement || RequireExplicitCastInterface;

                ForEachStatement = forEachStatement;
            }

            public bool RequireExplicitCastInterface => ExplicitCastInterface != null;

            public string CollectionNameSuggestion { get; }
            public string CountName { get; }
            public ITypeSymbol ExplicitCastInterface { get; }
            public ITypeSymbol ForEachElementType { get; }
            public bool RequireCollectionStatement { get; }
            public TForEachStatement ForEachStatement { get; }
        }

        private class ForEachToForCodeAction : CodeAction.DocumentChangeAction
        {
            public ForEachToForCodeAction(
                string title,
                Func<CancellationToken, Task<Document>> createChangedDocument) : base(title, createChangedDocument)
            {
            }
        }
    }
}
