// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Test.Utilities;
using Roslyn.Test.Utilities;
using Xunit;


namespace Microsoft.CodeAnalysis.CSharp.UnitTests
{
    public partial class IOperationTests : SemanticModelTestBase
    {
        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IWhileUntilLoopStatement_DoWhileLoopsTest()
        {
            string source = @"
class Program
{
    static void Main()
    {
        int[] ids = new int[] { 6, 7, 8, 10 };
        int sum = 0;
        int i = 0;
        /*<bind>*/do
        {
            sum += ids[i];
            i++;
        } while (i < 4);/*</bind>*/

        System.Console.WriteLine(sum);
    }
}
";
            string expectedOperationTree = @"
IDoLoopStatement (DoLoopKind: DoWhileBottomLoop) (LoopKind.Do) ([3] OperationKind.LoopStatement) (Syntax: DoStatement, 'do ... le (i < 4);') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([1] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i < 4')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 4) (Syntax: NumericLiteralExpression, '4')
  IgnoredCondition: 
    null
  Body: 
    IBlockStatement (2 statements) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'sum += ids[i];')
        Expression: 
          ICompoundAssignmentExpression (BinaryOperatorKind.Add) ([0] OperationKind.CompoundAssignmentExpression, Type: System.Int32) (Syntax: AddAssignmentExpression, 'sum += ids[i]')
            Left: 
              ILocalReferenceExpression: sum ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'sum')
            Right: 
              IArrayElementReferenceExpression ([1] OperationKind.ArrayElementReferenceExpression, Type: System.Int32) (Syntax: ElementAccessExpression, 'ids[i]')
                Array reference: 
                  ILocalReferenceExpression: ids ([0] OperationKind.LocalReferenceExpression, Type: System.Int32[]) (Syntax: IdentifierName, 'ids')
                Indices(1):
                  ILocalReferenceExpression: i ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'i++;')
        Expression: 
          IIncrementOrDecrementExpression (Postfix) ([0] OperationKind.IncrementExpression, Type: System.Int32) (Syntax: PostIncrementExpression, 'i++')
            Target: 
              ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<DoStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IWhileUntilLoopStatement_WhileLoopsTest()
        {
            string source = @"
class Program
{
    static int SumWhile()
    {
        //
        // Sum numbers 0 .. 4
        //
        int sum = 0;
        int i = 0;
        /*<bind>*/while (i < 5)
        {
            sum += i;
            i++;
        }/*</bind>*/
        return sum;
    }
}
";
            string expectedOperationTree = @"
IWhileLoopStatement (LoopKind.While) ([2] OperationKind.LoopStatement) (Syntax: WhileStatement, 'while (i <  ... }') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i < 5')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
  Body: 
    IBlockStatement (2 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'sum += i;')
        Expression: 
          ICompoundAssignmentExpression (BinaryOperatorKind.Add) ([0] OperationKind.CompoundAssignmentExpression, Type: System.Int32) (Syntax: AddAssignmentExpression, 'sum += i')
            Left: 
              ILocalReferenceExpression: sum ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'sum')
            Right: 
              ILocalReferenceExpression: i ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'i++;')
        Expression: 
          IIncrementOrDecrementExpression (Postfix) ([0] OperationKind.IncrementExpression, Type: System.Int32) (Syntax: PostIncrementExpression, 'i++')
            Target: 
              ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<WhileStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IWhileUntilLoopStatement_WhileConditionTrue()
        {
            string source = @"
using System;

class Program
{
    static void Main()
    {
        int index = 0;
        bool condition = true;
        /*<bind>*/while (condition)
        {
            int value = ++index;
            if (value > 10)
            {
                condition = false;
            }
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IWhileLoopStatement (LoopKind.While) ([2] OperationKind.LoopStatement) (Syntax: WhileStatement, 'while (cond ... }') (Parent: BlockStatement)
  Condition: 
    ILocalReferenceExpression: condition ([0] OperationKind.LocalReferenceExpression, Type: System.Boolean) (Syntax: IdentifierName, 'condition')
  Body: 
    IBlockStatement (2 statements, 1 locals) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      Locals: Local_1: System.Int32 value
      IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'int value = ++index;')
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'value = ++index')
          Variables: Local_1: System.Int32 value
          Initializer: 
            IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= ++index')
              IIncrementOrDecrementExpression (Prefix) ([0] OperationKind.IncrementExpression, Type: System.Int32) (Syntax: PreIncrementExpression, '++index')
                Target: 
                  ILocalReferenceExpression: index ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'index')
      IIfStatement ([1] OperationKind.IfStatement) (Syntax: IfStatement, 'if (value > ... }')
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'value > 10')
            Left: 
              ILocalReferenceExpression: value ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'value')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
        IfTrue: 
          IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'condition = false;')
              Expression: 
                ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Boolean) (Syntax: SimpleAssignmentExpression, 'condition = false')
                  Left: 
                    ILocalReferenceExpression: condition ([0] OperationKind.LocalReferenceExpression, Type: System.Boolean) (Syntax: IdentifierName, 'condition')
                  Right: 
                    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Boolean, Constant: False) (Syntax: FalseLiteralExpression, 'false')
        IfFalse: 
          null
";
            VerifyOperationTreeForTest<WhileStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IWhileUntilLoopStatement_WhileWithBreak()
        {
            string source = @"
using System;

class Program
{
    static void Main()
    {
        int index = 0;
        /*<bind>*/while (true)
        {
            int value = ++index;
            if (value > 5)
            {
                Console.WriteLine(""While-loop break"");
                break;
            }
            Console.WriteLine(""While-loop statement"");
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IWhileLoopStatement (LoopKind.While) ([1] OperationKind.LoopStatement) (Syntax: WhileStatement, 'while (true ... }') (Parent: BlockStatement)
  Condition: 
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Boolean, Constant: True) (Syntax: TrueLiteralExpression, 'true')
  Body: 
    IBlockStatement (3 statements, 1 locals) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      Locals: Local_1: System.Int32 value
      IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'int value = ++index;')
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'value = ++index')
          Variables: Local_1: System.Int32 value
          Initializer: 
            IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= ++index')
              IIncrementOrDecrementExpression (Prefix) ([0] OperationKind.IncrementExpression, Type: System.Int32) (Syntax: PreIncrementExpression, '++index')
                Target: 
                  ILocalReferenceExpression: index ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'index')
      IIfStatement ([1] OperationKind.IfStatement) (Syntax: IfStatement, 'if (value > ... }')
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'value > 5')
            Left: 
              ILocalReferenceExpression: value ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'value')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
        IfTrue: 
          IBlockStatement (2 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... op break"");')
              Expression: 
                IInvocationExpression (void System.Console.WriteLine(System.String value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... oop break"")')
                  Instance Receiver: 
                    null
                  Arguments(1):
                    IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, '""While-loop break""')
                      ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: ""While-loop break"") (Syntax: StringLiteralExpression, '""While-loop break""')
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            IBranchStatement (BranchKind.Break) ([1] OperationKind.BranchStatement) (Syntax: BreakStatement, 'break;')
        IfFalse: 
          null
      IExpressionStatement ([2] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... tatement"");')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.String value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... statement"")')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, '""While-loop statement""')
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: ""While-loop statement"") (Syntax: StringLiteralExpression, '""While-loop statement""')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            VerifyOperationTreeForTest<WhileStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IWhileUntilLoopStatement_WhileWithThrow()
        {
            string source = @"
using System;

class Program
{
    static void Main()
    {
        int index = 0;
        /*<bind>*/while (true)
        {
            int value = ++index;
            if (value > 100)
            {
                throw new Exception(""Never hit"");
            }
            Console.WriteLine(""While-loop statement"");
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IWhileLoopStatement (LoopKind.While) ([1] OperationKind.LoopStatement) (Syntax: WhileStatement, 'while (true ... }') (Parent: BlockStatement)
  Condition: 
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Boolean, Constant: True) (Syntax: TrueLiteralExpression, 'true')
  Body: 
    IBlockStatement (3 statements, 1 locals) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      Locals: Local_1: System.Int32 value
      IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'int value = ++index;')
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'value = ++index')
          Variables: Local_1: System.Int32 value
          Initializer: 
            IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= ++index')
              IIncrementOrDecrementExpression (Prefix) ([0] OperationKind.IncrementExpression, Type: System.Int32) (Syntax: PreIncrementExpression, '++index')
                Target: 
                  ILocalReferenceExpression: index ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'index')
      IIfStatement ([1] OperationKind.IfStatement) (Syntax: IfStatement, 'if (value > ... }')
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'value > 100')
            Left: 
              ILocalReferenceExpression: value ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'value')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 100) (Syntax: NumericLiteralExpression, '100')
        IfTrue: 
          IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ThrowStatement, 'throw new E ... ever hit"");')
              Expression: 
                IThrowExpression ([0] OperationKind.ThrowExpression, Type: System.Exception) (Syntax: ThrowStatement, 'throw new E ... ever hit"");')
                  IObjectCreationExpression (Constructor: System.Exception..ctor(System.String message)) ([0] OperationKind.ObjectCreationExpression, Type: System.Exception) (Syntax: ObjectCreationExpression, 'new Excepti ... Never hit"")')
                    Arguments(1):
                      IArgument (ArgumentKind.Explicit, Matching Parameter: message) ([0] OperationKind.Argument) (Syntax: Argument, '""Never hit""')
                        ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: ""Never hit"") (Syntax: StringLiteralExpression, '""Never hit""')
                        InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                        OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    Initializer: 
                      null
        IfFalse: 
          null
      IExpressionStatement ([2] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ... tatement"");')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.String value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... statement"")')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, '""While-loop statement""')
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: ""While-loop statement"") (Syntax: StringLiteralExpression, '""While-loop statement""')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            VerifyOperationTreeForTest<WhileStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IWhileUntilLoopStatement_WhileWithAssignment()
        {
            string source = @"
using System;

class Program
{
    static void Main()
    {
        int value = 4;
        int i;
        /*<bind>*/while ((i = value) >= 0)
        {
             Console.WriteLine(""While {0} {1}"", i, value);
            value--;
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IWhileLoopStatement (LoopKind.While) ([2] OperationKind.LoopStatement) (Syntax: WhileStatement, 'while ((i = ... }') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.GreaterThanOrEqual) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanOrEqualExpression, '(i = value) >= 0')
      Left: 
        ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = value')
          Left: 
            ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
          Right: 
            ILocalReferenceExpression: value ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'value')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  Body: 
    IBlockStatement (2 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'Console.Wri ...  i, value);')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.String format, System.Object arg0, System.Object arg1)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'Console.Wri ... , i, value)')
            Instance Receiver: 
              null
            Arguments(3):
              IArgument (ArgumentKind.Explicit, Matching Parameter: format) ([0] OperationKind.Argument) (Syntax: Argument, '""While {0} {1}""')
                ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.String, Constant: ""While {0} {1}"") (Syntax: StringLiteralExpression, '""While {0} {1}""')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              IArgument (ArgumentKind.Explicit, Matching Parameter: arg0) ([1] OperationKind.Argument) (Syntax: Argument, 'i')
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: IdentifierName, 'i')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
              IArgument (ArgumentKind.Explicit, Matching Parameter: arg1) ([2] OperationKind.Argument) (Syntax: Argument, 'value')
                IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: IdentifierName, 'value')
                  Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  Operand: 
                    ILocalReferenceExpression: value ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'value')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'value--;')
        Expression: 
          IIncrementOrDecrementExpression (Postfix) ([0] OperationKind.DecrementExpression, Type: System.Int32) (Syntax: PostDecrementExpression, 'value--')
            Target: 
              ILocalReferenceExpression: value ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'value')
";
            VerifyOperationTreeForTest<WhileStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IWhileUntilLoopStatement_WhileInvalidCondition()
        {
            string source = @"
class Program
{
    static void Main()
    {
        int number = 10;
        /*<bind>*/while (number)
        {
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IWhileLoopStatement (LoopKind.While) ([1] OperationKind.LoopStatement, IsInvalid) (Syntax: WhileStatement, 'while (numb ... }') (Parent: BlockStatement)
  Condition: 
    IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Boolean, IsInvalid, IsImplicit) (Syntax: IdentifierName, 'number')
      Conversion: CommonConversion (Exists: False, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      Operand: 
        ILocalReferenceExpression: number ([0] OperationKind.LocalReferenceExpression, Type: System.Int32, IsInvalid) (Syntax: IdentifierName, 'number')
  Body: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
";
            VerifyOperationTreeForTest<WhileStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IWhileUntilLoopStatement_WhileWithReturn()
        {
            string source = @"
class Program
{
    static void Main()
    {
        System.Console.WriteLine(GetFirstEvenNumber(33));
    }
    public static int GetFirstEvenNumber(int number)
    {
        /*<bind>*/while (true)
        {
            if ((number % 2) == 0)
            {
                return number;
            }
            number++;

        }/*</bind>*/
    }
}
";

            string expectedOperationTree = @"
IWhileLoopStatement (LoopKind.While) ([0] OperationKind.LoopStatement) (Syntax: WhileStatement, 'while (true ... }') (Parent: BlockStatement)
  Condition: 
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Boolean, Constant: True) (Syntax: TrueLiteralExpression, 'true')
  Body: 
    IBlockStatement (2 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IIfStatement ([0] OperationKind.IfStatement) (Syntax: IfStatement, 'if ((number ... }')
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.Equals) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: EqualsExpression, '(number % 2) == 0')
            Left: 
              IBinaryOperatorExpression (BinaryOperatorKind.Remainder) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: ModuloExpression, 'number % 2')
                Left: 
                  IParameterReferenceExpression: number ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'number')
                Right: 
                  ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
        IfTrue: 
          IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
            IReturnStatement ([0] OperationKind.ReturnStatement) (Syntax: ReturnStatement, 'return number;')
              ReturnedValue: 
                IParameterReferenceExpression: number ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'number')
        IfFalse: 
          null
      IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'number++;')
        Expression: 
          IIncrementOrDecrementExpression (Postfix) ([0] OperationKind.IncrementExpression, Type: System.Int32) (Syntax: PostIncrementExpression, 'number++')
            Target: 
              IParameterReferenceExpression: number ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'number')
";
            VerifyOperationTreeForTest<WhileStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IWhileUntilLoopStatement_WhileWithGoto()
        {
            string source = @"
class Program
{
    static void Main()
    {
        System.Console.WriteLine(GetFirstEvenNumber(33));
    }
    public static int GetFirstEvenNumber(int number)
    {
        /*<bind>*/while (true)
        {
            if ((number % 2) == 0)
            {
                goto Even;
            }
            number++;
        Even:
            return number;
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IWhileLoopStatement (LoopKind.While) ([0] OperationKind.LoopStatement) (Syntax: WhileStatement, 'while (true ... }') (Parent: BlockStatement)
  Condition: 
    ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Boolean, Constant: True) (Syntax: TrueLiteralExpression, 'true')
  Body: 
    IBlockStatement (3 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IIfStatement ([0] OperationKind.IfStatement) (Syntax: IfStatement, 'if ((number ... }')
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.Equals) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: EqualsExpression, '(number % 2) == 0')
            Left: 
              IBinaryOperatorExpression (BinaryOperatorKind.Remainder) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: ModuloExpression, 'number % 2')
                Left: 
                  IParameterReferenceExpression: number ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'number')
                Right: 
                  ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
        IfTrue: 
          IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
            IBranchStatement (BranchKind.GoTo, Label: Even) ([0] OperationKind.BranchStatement) (Syntax: GotoStatement, 'goto Even;')
        IfFalse: 
          null
      IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'number++;')
        Expression: 
          IIncrementOrDecrementExpression (Postfix) ([0] OperationKind.IncrementExpression, Type: System.Int32) (Syntax: PostIncrementExpression, 'number++')
            Target: 
              IParameterReferenceExpression: number ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'number')
      ILabeledStatement (Label: Even) ([2] OperationKind.LabeledStatement) (Syntax: LabeledStatement, 'Even: ... urn number;')
        Statement: 
          IReturnStatement ([0] OperationKind.ReturnStatement) (Syntax: ReturnStatement, 'return number;')
            ReturnedValue: 
              IParameterReferenceExpression: number ([0] OperationKind.ParameterReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'number')
";
            VerifyOperationTreeForTest<WhileStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IWhileUntilLoopStatement_WhileMissingCondition()
        {
            string source = @"
class Program
{
    static void Main()
    {
        int index = 0;
        bool condition = true;
        /*<bind>*/while ()
        {
            int value = ++index;
            if (value > 100)
            {
                condition = false;
            }
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IWhileLoopStatement (LoopKind.While) ([2] OperationKind.LoopStatement, IsInvalid) (Syntax: WhileStatement, 'while () ... }') (Parent: BlockStatement)
  Condition: 
    IInvalidExpression ([0] OperationKind.InvalidExpression, Type: null, IsInvalid) (Syntax: IdentifierName, '')
      Children(0)
  Body: 
    IBlockStatement (2 statements, 1 locals) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      Locals: Local_1: System.Int32 value
      IVariableDeclarationStatement (1 declarations) ([0] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'int value = ++index;')
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'value = ++index')
          Variables: Local_1: System.Int32 value
          Initializer: 
            IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= ++index')
              IIncrementOrDecrementExpression (Prefix) ([0] OperationKind.IncrementExpression, Type: System.Int32) (Syntax: PreIncrementExpression, '++index')
                Target: 
                  ILocalReferenceExpression: index ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'index')
      IIfStatement ([1] OperationKind.IfStatement) (Syntax: IfStatement, 'if (value > ... }')
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'value > 100')
            Left: 
              ILocalReferenceExpression: value ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'value')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 100) (Syntax: NumericLiteralExpression, '100')
        IfTrue: 
          IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'condition = false;')
              Expression: 
                ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Boolean) (Syntax: SimpleAssignmentExpression, 'condition = false')
                  Left: 
                    ILocalReferenceExpression: condition ([0] OperationKind.LocalReferenceExpression, Type: System.Boolean) (Syntax: IdentifierName, 'condition')
                  Right: 
                    ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Boolean, Constant: False) (Syntax: FalseLiteralExpression, 'false')
        IfFalse: 
          null
";
            VerifyOperationTreeForTest<WhileStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IWhileUntilLoopStatement_WhileMissingStatement()
        {
            string source = @"
class ContinueTest
{
    static void Main()
    {
        int i = 0;
        /*<bind>*/while(i <= 10)
        {

        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IWhileLoopStatement (LoopKind.While) ([1] OperationKind.LoopStatement) (Syntax: WhileStatement, 'while(i <=  ... }') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThanOrEqual) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanOrEqualExpression, 'i <= 10')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
  Body: 
    IBlockStatement (0 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
";
            VerifyOperationTreeForTest<WhileStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IWhileUntilLoopStatement_WhileWithContinue()
        {
            string source = @"
class ContinueTest
{
    static void Main()
    {
        int i = 0;
        /*<bind>*/while(i <= 10)
        {
            i++;
            if (i < 9)
            {
                continue;
            }
            System.Console.WriteLine(i);
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IWhileLoopStatement (LoopKind.While) ([1] OperationKind.LoopStatement) (Syntax: WhileStatement, 'while(i <=  ... }') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThanOrEqual) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanOrEqualExpression, 'i <= 10')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
  Body: 
    IBlockStatement (3 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'i++;')
        Expression: 
          IIncrementOrDecrementExpression (Postfix) ([0] OperationKind.IncrementExpression, Type: System.Int32) (Syntax: PostIncrementExpression, 'i++')
            Target: 
              ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      IIfStatement ([1] OperationKind.IfStatement) (Syntax: IfStatement, 'if (i < 9) ... }')
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i < 9')
            Left: 
              ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 9) (Syntax: NumericLiteralExpression, '9')
        IfTrue: 
          IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
            IBranchStatement (BranchKind.Continue) ([0] OperationKind.BranchStatement) (Syntax: ContinueStatement, 'continue;')
        IfFalse: 
          null
      IExpressionStatement ([2] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... iteLine(i);')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.Int32 value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(i)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'i')
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            VerifyOperationTreeForTest<WhileStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IWhileUntilLoopStatement_WhileNested()
        {
            string source = @"
class Test
{
    static void Main()
    {
        int i = 0;
        /*<bind>*/while(i<10)
        {
            i++;
            int j = 0;
            while (j < 10)
            {
                j++;
                System.Console.WriteLine(j);
            }
            System.Console.WriteLine(i);
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IWhileLoopStatement (LoopKind.While) ([1] OperationKind.LoopStatement) (Syntax: WhileStatement, 'while(i<10) ... }') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i<10')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
  Body: 
    IBlockStatement (4 statements, 1 locals) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      Locals: Local_1: System.Int32 j
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'i++;')
        Expression: 
          IIncrementOrDecrementExpression (Postfix) ([0] OperationKind.IncrementExpression, Type: System.Int32) (Syntax: PostIncrementExpression, 'i++')
            Target: 
              ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      IVariableDeclarationStatement (1 declarations) ([1] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'int j = 0;')
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'j = 0')
          Variables: Local_1: System.Int32 j
          Initializer: 
            IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
              ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
      IWhileLoopStatement (LoopKind.While) ([2] OperationKind.LoopStatement) (Syntax: WhileStatement, 'while (j <  ... }')
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'j < 10')
            Left: 
              ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
        Body: 
          IBlockStatement (2 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'j++;')
              Expression: 
                IIncrementOrDecrementExpression (Postfix) ([0] OperationKind.IncrementExpression, Type: System.Int32) (Syntax: PostIncrementExpression, 'j++')
                  Target: 
                    ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
            IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... iteLine(j);')
              Expression: 
                IInvocationExpression (void System.Console.WriteLine(System.Int32 value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(j)')
                  Instance Receiver: 
                    null
                  Arguments(1):
                    IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'j')
                      ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      IExpressionStatement ([3] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... iteLine(i);')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.Int32 value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(i)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'i')
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            VerifyOperationTreeForTest<WhileStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IWhileUntilLoopStatement_WhileChangeOuterInnerValue()
        {
            string source = @"
class Test
{
    static void Main()
    {
        int i = 0;
        /*<bind>*/while(i<10)
        {
            i++;
            int j = 0;
            while (j < 10)
            {
                j++;
                i = i + j;
                System.Console.WriteLine(j);
            }
            System.Console.WriteLine(i);
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IWhileLoopStatement (LoopKind.While) ([1] OperationKind.LoopStatement) (Syntax: WhileStatement, 'while(i<10) ... }') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'i<10')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
  Body: 
    IBlockStatement (4 statements, 1 locals) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      Locals: Local_1: System.Int32 j
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'i++;')
        Expression: 
          IIncrementOrDecrementExpression (Postfix) ([0] OperationKind.IncrementExpression, Type: System.Int32) (Syntax: PostIncrementExpression, 'i++')
            Target: 
              ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      IVariableDeclarationStatement (1 declarations) ([1] OperationKind.VariableDeclarationStatement) (Syntax: LocalDeclarationStatement, 'int j = 0;')
        IVariableDeclaration (1 variables) ([0] OperationKind.VariableDeclaration) (Syntax: VariableDeclarator, 'j = 0')
          Variables: Local_1: System.Int32 j
          Initializer: 
            IVariableInitializer ([0] OperationKind.VariableInitializer) (Syntax: EqualsValueClause, '= 0')
              ILiteralExpression ([0] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
      IWhileLoopStatement (LoopKind.While) ([2] OperationKind.LoopStatement) (Syntax: WhileStatement, 'while (j <  ... }')
        Condition: 
          IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, 'j < 10')
            Left: 
              ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 10) (Syntax: NumericLiteralExpression, '10')
        Body: 
          IBlockStatement (3 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'j++;')
              Expression: 
                IIncrementOrDecrementExpression (Postfix) ([0] OperationKind.IncrementExpression, Type: System.Int32) (Syntax: PostIncrementExpression, 'j++')
                  Target: 
                    ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
            IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'i = i + j;')
              Expression: 
                ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Int32) (Syntax: SimpleAssignmentExpression, 'i = i + j')
                  Left: 
                    ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                  Right: 
                    IBinaryOperatorExpression (BinaryOperatorKind.Add) ([1] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: AddExpression, 'i + j')
                      Left: 
                        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                      Right: 
                        ILocalReferenceExpression: j ([1] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
            IExpressionStatement ([2] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... iteLine(j);')
              Expression: 
                IInvocationExpression (void System.Console.WriteLine(System.Int32 value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(j)')
                  Instance Receiver: 
                    null
                  Arguments(1):
                    IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'j')
                      ILocalReferenceExpression: j ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'j')
                      InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      IExpressionStatement ([3] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... iteLine(i);')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.Int32 value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(i)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'i')
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            VerifyOperationTreeForTest<WhileStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IWhileUntilLoopStatement_WhileWithDynamic()
        {
            string source = @"
class C
{
    static void Main(string[] args)
    {
        dynamic d = new MyWhile();
        d.Initialize(5);
        /*<bind>*/while (d.Done)
        {
            d.Next();
        }/*</bind>*/
    }
}

public class MyWhile
{
    int index;
    int max;
    public void Initialize(int max)
    {
        index = 0;
        this.max = max;
        System.Console.WriteLine(""Initialize"");
    }
    public bool Done
    {
        get
        {
            System.Console.WriteLine(""Done"");
            return index < max;
        }
    }
    public void Next()
    {
        index = index + 1;
        System.Console.WriteLine(""Next"");
    }
}
";
            string expectedOperationTree = @"
IWhileLoopStatement (LoopKind.While) ([2] OperationKind.LoopStatement) (Syntax: WhileStatement, 'while (d.Do ... }') (Parent: BlockStatement)
  Condition: 
    IUnaryOperatorExpression (UnaryOperatorKind.True) ([0] OperationKind.UnaryOperatorExpression, Type: System.Boolean, IsImplicit) (Syntax: SimpleMemberAccessExpression, 'd.Done')
      Operand: 
        IDynamicMemberReferenceExpression (Member Name: ""Done"", Containing Type: null) ([0] OperationKind.DynamicMemberReferenceExpression, Type: dynamic) (Syntax: SimpleMemberAccessExpression, 'd.Done')
          Type Arguments(0)
          Instance Receiver: 
            ILocalReferenceExpression: d ([0] OperationKind.LocalReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'd')
  Body: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'd.Next();')
        Expression: 
          IDynamicInvocationExpression ([0] OperationKind.DynamicInvocationExpression, Type: dynamic) (Syntax: InvocationExpression, 'd.Next()')
            Expression: 
              IDynamicMemberReferenceExpression (Member Name: ""Next"", Containing Type: null) ([0] OperationKind.DynamicMemberReferenceExpression, Type: dynamic) (Syntax: SimpleMemberAccessExpression, 'd.Next')
                Type Arguments(0)
                Instance Receiver: 
                  ILocalReferenceExpression: d ([0] OperationKind.LocalReferenceExpression, Type: dynamic) (Syntax: IdentifierName, 'd')
            Arguments(0)
            ArgumentNames(0)
            ArgumentRefKinds(0)
";
            VerifyOperationTreeForTest<WhileStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IWhileUntilLoopStatement_WhileIncrementInCondition()
        {
            string source = @"
class Program
{
    static void Main(string[] args)
    {
        int i = 0;
        /*<bind>*/while ( ++i < 5)
        {
            System.Console.WriteLine(i);
        }/*</bind>*/
    }
}

";
            string expectedOperationTree = @"
IWhileLoopStatement (LoopKind.While) ([1] OperationKind.LoopStatement) (Syntax: WhileStatement, 'while ( ++i ... }') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.LessThan) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: LessThanExpression, '++i < 5')
      Left: 
        IIncrementOrDecrementExpression (Prefix) ([0] OperationKind.IncrementExpression, Type: System.Int32) (Syntax: PreIncrementExpression, '++i')
          Target: 
            ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 5) (Syntax: NumericLiteralExpression, '5')
  Body: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... iteLine(i);')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.Int32 value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... riteLine(i)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'i')
                ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
";
            VerifyOperationTreeForTest<WhileStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IWhileUntilLoopStatement_WhileInfiniteLoop()
        {
            string source = @"
class C
{
    static void Main(string[] args)
    {
        int i = 1;
        /*<bind>*/while (i > 0)
        {
            i++;
        }/*</bind>*/
    }
}";
            string expectedOperationTree = @"
IWhileLoopStatement (LoopKind.While) ([1] OperationKind.LoopStatement) (Syntax: WhileStatement, 'while (i >  ... }') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'i > 0')
      Left: 
        ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  Body: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'i++;')
        Expression: 
          IIncrementOrDecrementExpression (Postfix) ([0] OperationKind.IncrementExpression, Type: System.Int32) (Syntax: PostIncrementExpression, 'i++')
            Target: 
              ILocalReferenceExpression: i ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'i')
";
            VerifyOperationTreeForTest<WhileStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IWhileUntilLoopStatement_WhileConstantCheck()
        {
            string source = @"
class Program
{
    bool foo()
    {
        const bool b = true;
        /*<bind>*/while (b == b)
        {
            return b;
        }/*</bind>*/
    }
}
";
            string expectedOperationTree = @"
IWhileLoopStatement (LoopKind.While) ([1] OperationKind.LoopStatement) (Syntax: WhileStatement, 'while (b == ... }') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.Equals) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean, Constant: True) (Syntax: EqualsExpression, 'b == b')
      Left: 
        ILocalReferenceExpression: b ([0] OperationKind.LocalReferenceExpression, Type: System.Boolean, Constant: True) (Syntax: IdentifierName, 'b')
      Right: 
        ILocalReferenceExpression: b ([1] OperationKind.LocalReferenceExpression, Type: System.Boolean, Constant: True) (Syntax: IdentifierName, 'b')
  Body: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IReturnStatement ([0] OperationKind.ReturnStatement) (Syntax: ReturnStatement, 'return b;')
        ReturnedValue: 
          ILocalReferenceExpression: b ([0] OperationKind.LocalReferenceExpression, Type: System.Boolean, Constant: True) (Syntax: IdentifierName, 'b')
";
            VerifyOperationTreeForTest<WhileStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IWhileUntilLoopStatement_WhileWithTryCatch()
        {
            string source = @"
public class TryCatchFinally
{
    public void TryMethod()
    {
        sbyte x = 111, y;
        /*<bind>*/while (x-- > 0)
        {
            try
            {
                y = (sbyte)(x / 2);
            }
            finally
            {
                throw new System.Exception(); 
            }
        }/*</bind>*/
       
    }
}
";
            string expectedOperationTree = @"
IWhileLoopStatement (LoopKind.While) ([1] OperationKind.LoopStatement) (Syntax: WhileStatement, 'while (x--  ... }') (Parent: BlockStatement)
  Condition: 
    IBinaryOperatorExpression (BinaryOperatorKind.GreaterThan) ([0] OperationKind.BinaryOperatorExpression, Type: System.Boolean) (Syntax: GreaterThanExpression, 'x-- > 0')
      Left: 
        IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: PostDecrementExpression, 'x--')
          Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          Operand: 
            IIncrementOrDecrementExpression (Postfix) ([0] OperationKind.DecrementExpression, Type: System.SByte) (Syntax: PostDecrementExpression, 'x--')
              Target: 
                ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.SByte) (Syntax: IdentifierName, 'x')
      Right: 
        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 0) (Syntax: NumericLiteralExpression, '0')
  Body: 
    IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      ITryStatement ([0] OperationKind.TryStatement) (Syntax: TryStatement, 'try ... }')
        Body: 
          IBlockStatement (1 statements) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'y = (sbyte)(x / 2);')
              Expression: 
                ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.SByte) (Syntax: SimpleAssignmentExpression, 'y = (sbyte)(x / 2)')
                  Left: 
                    ILocalReferenceExpression: y ([0] OperationKind.LocalReferenceExpression, Type: System.SByte) (Syntax: IdentifierName, 'y')
                  Right: 
                    IConversionExpression (Explicit, TryCast: False, Unchecked) ([1] OperationKind.ConversionExpression, Type: System.SByte) (Syntax: CastExpression, '(sbyte)(x / 2)')
                      Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                      Operand: 
                        IBinaryOperatorExpression (BinaryOperatorKind.Divide) ([0] OperationKind.BinaryOperatorExpression, Type: System.Int32) (Syntax: DivideExpression, 'x / 2')
                          Left: 
                            IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Int32, IsImplicit) (Syntax: IdentifierName, 'x')
                              Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: True, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                              Operand: 
                                ILocalReferenceExpression: x ([0] OperationKind.LocalReferenceExpression, Type: System.SByte) (Syntax: IdentifierName, 'x')
                          Right: 
                            ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
        Catch clauses(0)
        Finally: 
          IBlockStatement (1 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
            IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ThrowStatement, 'throw new S ... xception();')
              Expression: 
                IThrowExpression ([0] OperationKind.ThrowExpression, Type: System.Exception) (Syntax: ThrowStatement, 'throw new S ... xception();')
                  IObjectCreationExpression (Constructor: System.Exception..ctor()) ([0] OperationKind.ObjectCreationExpression, Type: System.Exception) (Syntax: ObjectCreationExpression, 'new System.Exception()')
                    Arguments(0)
                    Initializer: 
                      null
";
            VerifyOperationTreeForTest<WhileStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IWhileUntilLoopStatement_WhileWithOutVar()
        {
            string source = @"
public class X
{
    public static void Main()
    {
        bool f = true;

        /*<bind>*/while (Dummy(f, TakeOutParam((f ? 1 : 2), out var x1), x1))
        {
            System.Console.WriteLine(x1);
            f = false;
        }/*</bind>*/
    }

    static bool Dummy(bool x, object y, object z)
    {
        System.Console.WriteLine(z);
        return x;
    }

    static bool TakeOutParam<T>(T y, out T x)
    {
        x = y;
        return true;
    }
}
";
            string expectedOperationTree = @"
IWhileLoopStatement (LoopKind.While) ([1] OperationKind.LoopStatement) (Syntax: WhileStatement, 'while (Dumm ... }') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 x1
  Condition: 
    IInvocationExpression (System.Boolean X.Dummy(System.Boolean x, System.Object y, System.Object z)) ([0] OperationKind.InvocationExpression, Type: System.Boolean) (Syntax: InvocationExpression, 'Dummy(f, Ta ... ar x1), x1)')
      Instance Receiver: 
        null
      Arguments(3):
        IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([0] OperationKind.Argument) (Syntax: Argument, 'f')
          ILocalReferenceExpression: f ([0] OperationKind.LocalReferenceExpression, Type: System.Boolean) (Syntax: IdentifierName, 'f')
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        IArgument (ArgumentKind.Explicit, Matching Parameter: y) ([1] OperationKind.Argument) (Syntax: Argument, 'TakeOutPara ... out var x1)')
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: InvocationExpression, 'TakeOutPara ... out var x1)')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IInvocationExpression (System.Boolean X.TakeOutParam<System.Int32>(System.Int32 y, out System.Int32 x)) ([0] OperationKind.InvocationExpression, Type: System.Boolean) (Syntax: InvocationExpression, 'TakeOutPara ... out var x1)')
                Instance Receiver: 
                  null
                Arguments(2):
                  IArgument (ArgumentKind.Explicit, Matching Parameter: y) ([0] OperationKind.Argument, IsImplicit) (Syntax: ConditionalExpression, 'f ? 1 : 2')
                    IConditionalExpression ([0] OperationKind.ConditionalExpression, Type: System.Int32) (Syntax: ConditionalExpression, 'f ? 1 : 2')
                      Condition: 
                        ILocalReferenceExpression: f ([0] OperationKind.LocalReferenceExpression, Type: System.Boolean) (Syntax: IdentifierName, 'f')
                      WhenTrue: 
                        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                      WhenFalse: 
                        ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
                    InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([1] OperationKind.Argument) (Syntax: Argument, 'out var x1')
                    IDeclarationExpression ([0] OperationKind.DeclarationExpression, Type: System.Int32) (Syntax: DeclarationExpression, 'var x1')
                      ILocalReferenceExpression: x1 (IsDeclaration: True) ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: SingleVariableDesignation, 'x1')
                    InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        IArgument (ArgumentKind.Explicit, Matching Parameter: z) ([2] OperationKind.Argument) (Syntax: Argument, 'x1')
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: IdentifierName, 'x1')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              ILocalReferenceExpression: x1 ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x1')
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  Body: 
    IBlockStatement (2 statements) ([1] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'System.Cons ... teLine(x1);')
        Expression: 
          IInvocationExpression (void System.Console.WriteLine(System.Int32 value)) ([0] OperationKind.InvocationExpression, Type: System.Void) (Syntax: InvocationExpression, 'System.Cons ... iteLine(x1)')
            Instance Receiver: 
              null
            Arguments(1):
              IArgument (ArgumentKind.Explicit, Matching Parameter: value) ([0] OperationKind.Argument) (Syntax: Argument, 'x1')
                ILocalReferenceExpression: x1 ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x1')
                InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
      IExpressionStatement ([1] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'f = false;')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Boolean) (Syntax: SimpleAssignmentExpression, 'f = false')
            Left: 
              ILocalReferenceExpression: f ([0] OperationKind.LocalReferenceExpression, Type: System.Boolean) (Syntax: IdentifierName, 'f')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Boolean, Constant: False) (Syntax: FalseLiteralExpression, 'false')
";
            VerifyOperationTreeForTest<WhileStatementSyntax>(source, expectedOperationTree);
        }

        [CompilerTrait(CompilerFeature.IOperation)]
        [Fact, WorkItem(17602, "https://github.com/dotnet/roslyn/issues/17602")]
        public void IWhileUntilLoopStatement_DoWithOutVar()
        {
            string source = @"
class X
{
    public static void Main()
    {
        bool f = true;

        /*<bind>*/do
        {
            f = false;
        } while (Dummy(f, TakeOutParam((f ? 1 : 2), out var x1), x1));/*</bind>*/
    }

    static bool Dummy(bool x, object y, object z)
    {
        System.Console.WriteLine(z);
        return x;
    }

    static bool TakeOutParam<T>(T y, out T x)
    {
        x = y;
        return true;
    }
}
";
            string expectedOperationTree = @"
IDoLoopStatement (DoLoopKind: DoWhileBottomLoop) (LoopKind.Do) ([1] OperationKind.LoopStatement) (Syntax: DoStatement, 'do ...  x1), x1));') (Parent: BlockStatement)
  Locals: Local_1: System.Int32 x1
  Condition: 
    IInvocationExpression (System.Boolean X.Dummy(System.Boolean x, System.Object y, System.Object z)) ([1] OperationKind.InvocationExpression, Type: System.Boolean) (Syntax: InvocationExpression, 'Dummy(f, Ta ... ar x1), x1)')
      Instance Receiver: 
        null
      Arguments(3):
        IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([0] OperationKind.Argument) (Syntax: Argument, 'f')
          ILocalReferenceExpression: f ([0] OperationKind.LocalReferenceExpression, Type: System.Boolean) (Syntax: IdentifierName, 'f')
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        IArgument (ArgumentKind.Explicit, Matching Parameter: y) ([1] OperationKind.Argument) (Syntax: Argument, 'TakeOutPara ... out var x1)')
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: InvocationExpression, 'TakeOutPara ... out var x1)')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              IInvocationExpression (System.Boolean X.TakeOutParam<System.Int32>(System.Int32 y, out System.Int32 x)) ([0] OperationKind.InvocationExpression, Type: System.Boolean) (Syntax: InvocationExpression, 'TakeOutPara ... out var x1)')
                Instance Receiver: 
                  null
                Arguments(2):
                  IArgument (ArgumentKind.Explicit, Matching Parameter: y) ([0] OperationKind.Argument, IsImplicit) (Syntax: ConditionalExpression, 'f ? 1 : 2')
                    IConditionalExpression ([0] OperationKind.ConditionalExpression, Type: System.Int32) (Syntax: ConditionalExpression, 'f ? 1 : 2')
                      Condition: 
                        ILocalReferenceExpression: f ([0] OperationKind.LocalReferenceExpression, Type: System.Boolean) (Syntax: IdentifierName, 'f')
                      WhenTrue: 
                        ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Int32, Constant: 1) (Syntax: NumericLiteralExpression, '1')
                      WhenFalse: 
                        ILiteralExpression ([2] OperationKind.LiteralExpression, Type: System.Int32, Constant: 2) (Syntax: NumericLiteralExpression, '2')
                    InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                  IArgument (ArgumentKind.Explicit, Matching Parameter: x) ([1] OperationKind.Argument) (Syntax: Argument, 'out var x1')
                    IDeclarationExpression ([0] OperationKind.DeclarationExpression, Type: System.Int32) (Syntax: DeclarationExpression, 'var x1')
                      ILocalReferenceExpression: x1 (IsDeclaration: True) ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: SingleVariableDesignation, 'x1')
                    InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
                    OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
        IArgument (ArgumentKind.Explicit, Matching Parameter: z) ([2] OperationKind.Argument) (Syntax: Argument, 'x1')
          IConversionExpression (Implicit, TryCast: False, Unchecked) ([0] OperationKind.ConversionExpression, Type: System.Object, IsImplicit) (Syntax: IdentifierName, 'x1')
            Conversion: CommonConversion (Exists: True, IsIdentity: False, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
            Operand: 
              ILocalReferenceExpression: x1 ([0] OperationKind.LocalReferenceExpression, Type: System.Int32) (Syntax: IdentifierName, 'x1')
          InConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
          OutConversion: CommonConversion (Exists: True, IsIdentity: True, IsNumeric: False, IsReference: False, IsUserDefined: False) (MethodSymbol: null)
  IgnoredCondition: 
    null
  Body: 
    IBlockStatement (1 statements) ([0] OperationKind.BlockStatement) (Syntax: Block, '{ ... }')
      IExpressionStatement ([0] OperationKind.ExpressionStatement) (Syntax: ExpressionStatement, 'f = false;')
        Expression: 
          ISimpleAssignmentExpression ([0] OperationKind.SimpleAssignmentExpression, Type: System.Boolean) (Syntax: SimpleAssignmentExpression, 'f = false')
            Left: 
              ILocalReferenceExpression: f ([0] OperationKind.LocalReferenceExpression, Type: System.Boolean) (Syntax: IdentifierName, 'f')
            Right: 
              ILiteralExpression ([1] OperationKind.LiteralExpression, Type: System.Boolean, Constant: False) (Syntax: FalseLiteralExpression, 'false')
";
            var expectedDiagnostics = DiagnosticDescription.None;

            VerifyOperationTreeAndDiagnosticsForTest<DoStatementSyntax>(source, expectedOperationTree, expectedDiagnostics);
        }
    }
}
