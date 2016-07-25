﻿' Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

Imports System.Collections.Immutable
Imports System.Composition
Imports System.Threading
Imports Microsoft.CodeAnalysis.Execution
Imports Microsoft.CodeAnalysis.Host.Mef

Namespace Microsoft.CodeAnalysis.VisualBasic.Execution
    <ExportLanguageService(GetType(ILanguageSpecificSerializationService), LanguageNames.VisualBasic), [Shared]>
    Friend Class VisualBasicLanguageSpecificSerializationService
        Inherits AbstractLanguageSpecificSerializationService

        Public Overrides Sub WriteTo(options As CompilationOptions, writer As ObjectWriter, cancellationToken As CancellationToken)
            WriteCompilationOptionsTo(options, writer, cancellationToken)

            Dim vbOptions = DirectCast(options, VisualBasicCompilationOptions)

            writer.WriteArray(vbOptions.GlobalImports.Select(Function(g) g.Name).ToArray())
            writer.WriteString(vbOptions.RootNamespace)
            writer.WriteInt32(vbOptions.OptionStrict)
            writer.WriteBoolean(vbOptions.OptionInfer)
            writer.WriteBoolean(vbOptions.OptionExplicit)
            writer.WriteBoolean(vbOptions.OptionCompareText)
            writer.WriteBoolean(vbOptions.EmbedVbCoreRuntime)
        End Sub

        Public Overrides Sub WriteTo(options As ParseOptions, writer As ObjectWriter, cancellationToken As CancellationToken)
            WriteParseOptionsTo(options, writer, cancellationToken)

            Dim vbOptions = DirectCast(options, VisualBasicParseOptions)
            writer.WriteInt32(vbOptions.LanguageVersion)

            writer.WriteInt32(vbOptions.PreprocessorSymbols.Length)
            For Each kv In vbOptions.PreprocessorSymbols
                writer.WriteString(kv.Key)

                ' all value here should be primitive types
                writer.WriteValue(kv.Value)
            Next
        End Sub

        Public Overrides Function ReadCompilationOptionsFrom(reader As ObjectReader, cancellationToken As CancellationToken) As CompilationOptions
            Dim outputKind As OutputKind
            Dim reportSuppressedDiagnostics As Boolean
            Dim moduleName As String = Nothing
            Dim mainTypeName As String = Nothing
            Dim scriptClassName As String = Nothing
            Dim optimizationLevel As OptimizationLevel
            Dim checkOverflow As Boolean
            Dim cryptoKeyContainer As String = Nothing
            Dim cryptoKeyFile As String = Nothing
            Dim cryptoPublicKey As ImmutableArray(Of Byte) = ImmutableArray(Of Byte).Empty
            Dim delaySign As Boolean?
            Dim platform As Platform
            Dim generalDiagnosticOption As ReportDiagnostic
            Dim warningLevel As Integer
            Dim specificDiagnosticOptions As IEnumerable(Of KeyValuePair(Of String, ReportDiagnostic)) = Nothing
            Dim concurrentBuild As Boolean
            Dim deterministic As Boolean
            Dim publicSign As Boolean
            Dim xmlReferenceResolver As XmlReferenceResolver = Nothing
            Dim sourceReferenceResolver As SourceReferenceResolver = Nothing
            Dim metadataReferenceResolver As MetadataReferenceResolver = Nothing
            Dim assemblyIdentityComparer As AssemblyIdentityComparer = Nothing
            Dim strongNameProvider As StrongNameProvider = Nothing

            ReadCompilationOptionsFrom(reader, outputKind, reportSuppressedDiagnostics, moduleName, mainTypeName, scriptClassName,
                optimizationLevel, checkOverflow, cryptoKeyContainer, cryptoKeyFile, cryptoPublicKey, delaySign,
                platform, generalDiagnosticOption, warningLevel, specificDiagnosticOptions, concurrentBuild, deterministic,
                publicSign, xmlReferenceResolver, sourceReferenceResolver, metadataReferenceResolver, assemblyIdentityComparer, strongNameProvider,
                cancellationToken)

            Dim globalImports = GlobalImport.Parse(reader.ReadArray(Of String)())
            Dim rootNamespace = reader.ReadString()
            Dim optionStrict = CType(reader.ReadInt32(), OptionStrict)
            Dim optionInfer = reader.ReadBoolean()
            Dim optionExplicit = reader.ReadBoolean()
            Dim optionCompareText = reader.ReadBoolean()
            Dim embedVbCoreRuntime = reader.ReadBoolean()

            Return New VisualBasicCompilationOptions(outputKind, moduleName, mainTypeName, scriptClassName,
                                                     globalImports, rootNamespace, optionStrict, optionInfer, optionExplicit,
                                                     optionCompareText, Nothing,
                                                     embedVbCoreRuntime, optimizationLevel, checkOverflow,
                                                     cryptoKeyContainer, cryptoKeyFile, cryptoPublicKey, delaySign,
                                                     platform, generalDiagnosticOption, specificDiagnosticOptions, concurrentBuild, deterministic,
                                                     xmlReferenceResolver, sourceReferenceResolver, metadataReferenceResolver, assemblyIdentityComparer, strongNameProvider,
                                                     publicSign, reportSuppressedDiagnostics)
        End Function

        Public Overrides Function ReadParseOptionsFrom(reader As ObjectReader, cancellationToken As CancellationToken) As ParseOptions
            Dim kind As SourceCodeKind
            Dim documentationMode As DocumentationMode
            Dim features As IEnumerable(Of KeyValuePair(Of String, String)) = Nothing
            ReadParseOptionsFrom(reader, kind, documentationMode, features, cancellationToken)

            Dim languageVersion = DirectCast(reader.ReadInt32(), LanguageVersion)

            Dim count = reader.ReadInt32()
            Dim builder = ImmutableArray.CreateBuilder(Of KeyValuePair(Of String, Object))
            For i = 0 To count - 1
                Dim key = reader.ReadString()
                Dim value = reader.ReadValue()
                builder.Add(KeyValuePair.Create(key, value))
            Next
            Dim options = New VisualBasicParseOptions(languageVersion, documentationMode, kind, builder.ToImmutable())
            Return options.WithFeatures(features)
        End Function
    End Class
End Namespace