﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System.Collections.Immutable;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CodeRefactorings;
using Microsoft.CodeAnalysis.CSharp.ForeachToFor;
using Microsoft.CodeAnalysis.Editor.CSharp.UnitTests.CodeRefactorings;
using Roslyn.Test.Utilities;
using Xunit;

namespace Microsoft.CodeAnalysis.Editor.CSharp.UnitTests.ForeachToFor
{
    public partial class ForeachToForTests : AbstractCSharpCodeActionTest
    {
        protected override CodeRefactoringProvider CreateCodeRefactoringProvider(Workspace workspace, TestParameters parameters)
            => new CSharpForEachToForCodeRefactoringProvider();

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task EmptyBlockBody()
        {
            var text = @"
class Test
{
    void Method()
    {
        var array = new int[] { 1, 3, 4 };
        foreach[||](var a in array)
        {
        }
    }
}
";
            var expected = @"
class Test
{
    void Method()
    {
        var array = new int[] { 1, 3, 4 };
        for (var {|Rename:i|} = 0; i < array.Length; i++)
        {
        }
    }
}
";
            await TestInRegularAndScriptAsync(text, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task EmptyBody()
        {
            var text = @"
class Test
{
    void Method()
    {
        var array = new int[] { 1, 3, 4 };
        foreach[||](var a in array) ;
    }
}
";
            var expected = @"
class Test
{
    void Method()
    {
        var array = new int[] { 1, 3, 4 };
        for (var {|Rename:i|} = 0; i < array.Length; i++) ;
    }
}
";
            await TestInRegularAndScriptAsync(text, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task Body()
        {
            var text = @"
class Test
{
    void Method()
    {
        var array = new int[] { 1, 3, 4 };
        foreach[||](var a in array) Console.WriteLine(a);
    }
}
";
            var expected = @"
class Test
{
    void Method()
    {
        var array = new int[] { 1, 3, 4 };
        for (var {|Rename:i|} = 0; i < array.Length; i++)
        {
            var a = array[i];
            Console.WriteLine(a);
        }
    }
}
";
            await TestInRegularAndScriptAsync(text, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task BlockBody()
        {
            var text = @"
class Test
{
    void Method()
    {
        var array = new int[] { 1, 3, 4 };
        foreach[||](var a in array)
        {
            Console.WriteLine(a);
        }
    }
}
";
            var expected = @"
class Test
{
    void Method()
    {
        var array = new int[] { 1, 3, 4 };
        for (var {|Rename:i|} = 0; i < array.Length; i++)
        {
            var a = array[i];
            Console.WriteLine(a);
        }
    }
}
";
            await TestInRegularAndScriptAsync(text, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task Comment()
        {
            var text = @"
class Test
{
    void Method()
    {
        var array = new int[] { 1, 3, 4 };
        /* comment */
        foreach[||](var a in array) /* comment */
        {
        }
    }
}
";
            var expected = @"
class Test
{
    void Method()
    {
        var array = new int[] { 1, 3, 4 };
        /* comment */
        for (var {|Rename:i|} = 0; i < array.Length; i++) /* comment */
        {
        }
    }
}
";
            await TestInRegularAndScriptAsync(text, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task Comment2()
        {
            var text = @"
class Test
{
    void Method()
    {
        var array = new int[] { 1, 3, 4 };
        foreach[||](var a in array)
        /* comment */
        {
        }/* comment */
    }
}
";
            var expected = @"
class Test
{
    void Method()
    {
        var array = new int[] { 1, 3, 4 };
        for (var {|Rename:i|} = 0; i < array.Length; i++)
        /* comment */
        {
        }/* comment */
    }
}
";
            await TestInRegularAndScriptAsync(text, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task Comment3()
        {
            var text = @"
class Test
{
    void Method()
    {
        var array = new int[] { 1, 3, 4 };
        foreach[||](var a in array) /* comment */;
    }
}
";
            var expected = @"
class Test
{
    void Method()
    {
        var array = new int[] { 1, 3, 4 };
        for (var {|Rename:i|} = 0; i < array.Length; i++) /* comment */;
    }
}
";
            await TestInRegularAndScriptAsync(text, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task Comment4()
        {
            var text = @"
class Test
{
    void Method()
    {
        var array = new int[] { 1, 3, 4 };
        foreach[||](var a in array) Console.WriteLine(a); // test
    }
}
";
            var expected = @"
class Test
{
    void Method()
    {
        var array = new int[] { 1, 3, 4 };
        for (var {|Rename:i|} = 0; i < array.Length; i++)
        {
            var a = array[i];
            Console.WriteLine(a); // test
        }
    }
}
";
            await TestInRegularAndScriptAsync(text, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task Comment5()
        {
            var text = @"
class Test
{
    void Method()
    {
        var array = new int[] { 1, 3, 4 };
        foreach [||] (var a in array) /* test */ Console.WriteLine(a); 
    }
}
";
            var expected = @"
class Test
{
    void Method()
    {
        var array = new int[] { 1, 3, 4 };
        for (var {|Rename:i|} = 0; i < array.Length; i++) /* test */
        {
            var a = array[i];
            Console.WriteLine(a);
        }
    }
}
";
            await TestInRegularAndScriptAsync(text, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task Comment6()
        {
            var text = @"
class Test
{
    void Method()
    {
        var array = new int[] { 1, 3, 4 };
        foreach [||] (var a in array) 
            /* test */ Console.WriteLine(a); 
    }
}
";
            var expected = @"
class Test
{
    void Method()
    {
        var array = new int[] { 1, 3, 4 };
        for (var {|Rename:i|} = 0; i < array.Length; i++)
        {
            var a = array[i];
            /* test */
            Console.WriteLine(a);
        }
    }
}
";
            await TestInRegularAndScriptAsync(text, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task Comment7()
        {
            var text = @"
class Test
{
    void Method()
    {
        // test
        foreach[||](var a in new int[] { 1, 3, 4 })
        {
        }
    }
}
";
            var expected = @"
class Test
{
    void Method()
    {
        // test
        var {|Rename:list|} = new int[] { 1, 3, 4 };
        for (var {|Rename:i|} = 0; i < list.Length; i++)
        {
        }
    }
}
";
            await TestInRegularAndScriptAsync(text, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task CommentNotSupported()
        {
            var text = @"
class Test
{
    void Method()
    {
        var array = new int[] { 1, 3, 4 };
        foreach [||] (var a /* test */ in array) ;
    }
}
";
            await TestMissingInRegularAndScriptAsync(text);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task CommentNotSupported2()
        {
            var text = @"
class Test
{
    void Method()
    {
        var array = new int[] { 1, 3, 4 };
        foreach [||] (/* test */ var a in array) ;
    }
}
";
            await TestMissingInRegularAndScriptAsync(text);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task CommentNotSupported3()
        {
            var text = @"
class Test
{
    void Method()
    {
        var array = new int[] { 1, 3, 4 };
        foreach [||] (var a in array /* test */) ;
    }
}
";
            await TestMissingInRegularAndScriptAsync(text);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task CollectionStatement()
        {
            var text = @"
class Test
{
    void Method()
    {
        foreach[||](var a in new int[] { 1, 3, 4 })
        {
            Console.WriteLine(a);
        }
    }
}
";
            var expected = @"
class Test
{
    void Method()
    {
        var {|Rename:list|} = new int[] { 1, 3, 4 };
        for (var {|Rename:i|} = 0; i < list.Length; i++)
        {
            var a = list[i];
            Console.WriteLine(a);
        }
    }
}
";
            await TestInRegularAndScriptAsync(text, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task CollectionConflict()
        {
            var text = @"
class Test
{
    void Method()
    {
        var list = 1;

        foreach[||](var a in new int[] { 1, 3, 4 })
        {
            Console.WriteLine(a);
        }
    }
}
";
            var expected = @"
class Test
{
    void Method()
    {
        var list = 1;

        var {|Rename:list1|} = new int[] { 1, 3, 4 };
        for (var {|Rename:i|} = 0; i < list1.Length; i++)
        {
            var a = list1[i];
            Console.WriteLine(a);
        }
    }
}
";
            await TestInRegularAndScriptAsync(text, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task IndexConflict()
        {
            var text = @"
class Test
{
    void Method()
    {
        var array = new int[] { 1, 3, 4 };
        foreach [||] (var a in array)
        {
            int i = 0;
        }
    }
}
";
            var expected = @"
class Test
{
    void Method()
    {
        var array = new int[] { 1, 3, 4 };
        for (var {|Rename:i1|} = 0; i1 < array.Length; i1++)
        {
            var a = array[i1];
            int i = 0;
        }
    }
}
";
            await TestInRegularAndScriptAsync(text, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task VariableWritten()
        {
            var text = @"
class Test
{
    void Method()
    {
        var array = new int[] { 1, 3, 4 };
        foreach [||] (var a in array)
        {
            a = 1;
        }
    }
}
";
            await TestMissingInRegularAndScriptAsync(text);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task WrongCaretPosition()
        {
            var text = @"
class Test
{
    void Method()
    {
        var array = new int[] { 1, 3, 4 };
        foreach (var a in array)
        {
            [||] 
        }
    }
}
";
            await TestMissingInRegularAndScriptAsync(text);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task WrongCaretPosition1()
        {
            var text = @"
class Test
{
    void Method()
    {
        var array = new int[] { 1, 3, 4 };
        [||] foreach (var a in array)
        {
        }
    }
}
";
            await TestMissingInRegularAndScriptAsync(text);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task WrongCaretPosition2()
        {
            var text = @"
class Test
{
    void Method()
    {
        var array = new int[] { 1, 3, 4 };
        foreach (var a in array) [||] 
        {
        }
    }
}
";
            await TestMissingInRegularAndScriptAsync(text);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task Field()
        {
            var text = @"
class Test
{
    int[] array = new int[] { 1, 3, 4 };

    void Method()
    {
        foreach [||] (var a in array)
        {
        }
    }
}
";
            var expected = @"
class Test
{
    int[] array = new int[] { 1, 3, 4 };

    void Method()
    {
        for (var {|Rename:i|} = 0; i < array.Length; i++)
        {
        }
    }
}
";
            await TestInRegularAndScriptAsync(text, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task Interface()
        {
            var text = @"
using System.Collections.Generic;

class Test
{
    void Method()
    {
        var array = (IList<int>)(new int[] { 1, 3, 4 });
        foreach[||] (var a in array)
        {
            Console.WriteLine(a);
        }
    }
}
";
            var expected = @"
using System.Collections.Generic;

class Test
{
    void Method()
    {
        var array = (IList<int>)(new int[] { 1, 3, 4 });
        for (var {|Rename:i|} = 0; i < array.Count; i++)
        {
            var a = array[i];
            Console.WriteLine(a);
        }
    }
}
";
            await TestInRegularAndScriptAsync(text, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task IListOfT()
        {
            var text = @"
using System.Collections.Generic;

class Test
{
    void Method()
    {
        var list = new List<int>();
        foreach [||](var a in list)
        {
            Console.WriteLine(a);
        }
    }
}
";
            var expected = @"
using System.Collections.Generic;

class Test
{
    void Method()
    {
        var list = new List<int>();
        for (var {|Rename:i|} = 0; i < list.Count; i++)
        {
            var a = list[i];
            Console.WriteLine(a);
        }
    }
}
";
            await TestInRegularAndScriptAsync(text, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task IReadOnlyListOfT()
        {
            var text = @"
using System.Collections;
using System.Collections.Generic;

class Test
{
    void Method()
    {
        var list = new ReadOnly<int>();
        foreach [||](var a in list)
        {
            Console.WriteLine(a);
        }

    }
}

class ReadOnly<T> : IReadOnlyList<T>
{
    public T this[int index] => throw new System.NotImplementedException();
    public int Count => throw new System.NotImplementedException();
    public IEnumerator<T> GetEnumerator() => throw new System.NotImplementedException();
    IEnumerator IEnumerable.GetEnumerator() => throw new System.NotImplementedException();
}
";
            var expected = @"
using System.Collections;
using System.Collections.Generic;

class Test
{
    void Method()
    {
        var list = new ReadOnly<int>();
        for (var {|Rename:i|} = 0; i < list.Count; i++)
        {
            var a = list[i];
            Console.WriteLine(a);
        }

    }
}

class ReadOnly<T> : IReadOnlyList<T>
{
    public T this[int index] => throw new System.NotImplementedException();
    public int Count => throw new System.NotImplementedException();
    public IEnumerator<T> GetEnumerator() => throw new System.NotImplementedException();
    IEnumerator IEnumerable.GetEnumerator() => throw new System.NotImplementedException();
}
";
            await TestInRegularAndScriptAsync(text, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task IList()
        {
            var text = @"
using System;
using System.Collections;

class Test
{
    void Method()
    {
        var list = new List();
        foreach [||](var a in list)
        {
            Console.WriteLine(a);
        }

    }
}

class List : IList
{
    public object this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool IsReadOnly => throw new NotImplementedException();
    public bool IsFixedSize => throw new NotImplementedException();
    public int Count => throw new NotImplementedException();
    public object SyncRoot => throw new NotImplementedException();
    public bool IsSynchronized => throw new NotImplementedException();
    public int Add(object value) => throw new NotImplementedException();
    public void Clear() => throw new NotImplementedException();
    public bool Contains(object value) => throw new NotImplementedException();
    public void CopyTo(Array array, int index) => throw new NotImplementedException();
    public IEnumerator GetEnumerator() => throw new NotImplementedException();
    public int IndexOf(object value) => throw new NotImplementedException();
    public void Insert(int index, object value) => throw new NotImplementedException();
    public void Remove(object value) => throw new NotImplementedException();
    public void RemoveAt(int index) => throw new NotImplementedException();
}
";
            var expected = @"
using System;
using System.Collections;

class Test
{
    void Method()
    {
        var list = new List();
        for (var {|Rename:i|} = 0; i < list.Count; i++)
        {
            var a = list[i];
            Console.WriteLine(a);
        }

    }
}

class List : IList
{
    public object this[int index] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public bool IsReadOnly => throw new NotImplementedException();
    public bool IsFixedSize => throw new NotImplementedException();
    public int Count => throw new NotImplementedException();
    public object SyncRoot => throw new NotImplementedException();
    public bool IsSynchronized => throw new NotImplementedException();
    public int Add(object value) => throw new NotImplementedException();
    public void Clear() => throw new NotImplementedException();
    public bool Contains(object value) => throw new NotImplementedException();
    public void CopyTo(Array array, int index) => throw new NotImplementedException();
    public IEnumerator GetEnumerator() => throw new NotImplementedException();
    public int IndexOf(object value) => throw new NotImplementedException();
    public void Insert(int index, object value) => throw new NotImplementedException();
    public void Remove(object value) => throw new NotImplementedException();
    public void RemoveAt(int index) => throw new NotImplementedException();
}
";
            await TestInRegularAndScriptAsync(text, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task ImmutableArray()
        {
            var text = @"
<Workspace>
    <Project Language=""C#"" AssemblyName=""Assembly1"" CommonReferences=""true"" CommonReferenceFacadeSystemRuntime = ""true"">
    <MetadataReference>" + typeof(ImmutableArray<>).Assembly.Location + @"</MetadataReference>
        <Document>
using System;
using System.Collections.Immutable;

class Test
{
    void Method()
    {
        var list = ImmutableArray.Create(1);
        foreach [||](var a in list)
        {
            Console.WriteLine(a);
        }
    }
}</Document>
    </Project>
</Workspace>";

            var expected = @"
using System;
using System.Collections.Immutable;

class Test
{
    void Method()
    {
        var list = ImmutableArray.Create(1);
        for (var {|Rename:i|} = 0; i < list.Length; i++)
        {
            var a = list[i];
            Console.WriteLine(a);
        }
    }
}";
            await TestInRegularAndScriptAsync(text, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task ExplicitInterface()
        {
            var text = @"
using System;
using System.Collections;
using System.Collections.Generic;

class Test
{
    void Method()
    {
        var list = new Explicit();
        foreach [||] (var a in list)
        {
            Console.WriteLine(a);
        }
    }
}

class Explicit : IReadOnlyList<int>
{
    int IReadOnlyList<int>.this[int index] => throw new NotImplementedException();
    int IReadOnlyCollection<int>.Count => throw new NotImplementedException();
    IEnumerator<int> IEnumerable<int>.GetEnumerator() => throw new NotImplementedException();
    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
}
";

            var expected = @"
using System;
using System.Collections;
using System.Collections.Generic;

class Test
{
    void Method()
    {
        var list = new Explicit();
        var {|Rename:list1|} = (IReadOnlyList<int>)list;
        for (var {|Rename:i|} = 0; i < list1.Count; i++)
        {
            var a = list1[i];
            Console.WriteLine(a);
        }
    }
}

class Explicit : IReadOnlyList<int>
{
    int IReadOnlyList<int>.this[int index] => throw new NotImplementedException();
    int IReadOnlyCollection<int>.Count => throw new NotImplementedException();
    IEnumerator<int> IEnumerable<int>.GetEnumerator() => throw new NotImplementedException();
    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
}
";
            await TestInRegularAndScriptAsync(text, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task DoubleExplicitInterface()
        {
            var text = @"
using System;
using System.Collections;
using System.Collections.Generic;

class Test
{
    void Method()
    {
        var list = new Explicit();
        foreach [||] (var a in list)
        {
            Console.WriteLine(a);
        }
    }
}

class Explicit : IReadOnlyList<int>, IReadOnlyList<string>
{
    int IReadOnlyList<int>.this[int index] => throw new NotImplementedException();
    int IReadOnlyCollection<int>.Count => throw new NotImplementedException();
    IEnumerator<int> IEnumerable<int>.GetEnumerator() => throw new NotImplementedException();
    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

    string IReadOnlyList<string>.this[int index] => throw new NotImplementedException();
    int IReadOnlyCollection<string>.Count => throw new NotImplementedException();
    IEnumerator<string> IEnumerable<string>.GetEnumerator() => throw new NotImplementedException();
}
";
            await TestMissingInRegularAndScriptAsync(text);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task DoubleExplicitInterfaceWithExplicitType()
        {
            var text = @"
using System;
using System.Collections;
using System.Collections.Generic;

class Test
{
    void Method()
    {
        var list = new Explicit();
        foreach [||] (int a in list)
        {
            Console.WriteLine(a);
        }
    }
}

class Explicit : IReadOnlyList<int>, IReadOnlyList<string>
{
    int IReadOnlyList<int>.this[int index] => throw new NotImplementedException();
    int IReadOnlyCollection<int>.Count => throw new NotImplementedException();
    IEnumerator<int> IEnumerable<int>.GetEnumerator() => throw new NotImplementedException();
    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

    string IReadOnlyList<string>.this[int index] => throw new NotImplementedException();
    int IReadOnlyCollection<string>.Count => throw new NotImplementedException();
    IEnumerator<string> IEnumerable<string>.GetEnumerator() => throw new NotImplementedException();
}
";

            var expected = @"
using System;
using System.Collections;
using System.Collections.Generic;

class Test
{
    void Method()
    {
        var list = new Explicit();
        var {|Rename:list1|} = (IReadOnlyList<int>)list;
        for (var {|Rename:i|} = 0; i < list1.Count; i++)
        {
            var a = list1[i];
            Console.WriteLine(a);
        }
    }
}

class Explicit : IReadOnlyList<int>, IReadOnlyList<string>
{
    int IReadOnlyList<int>.this[int index] => throw new NotImplementedException();
    int IReadOnlyCollection<int>.Count => throw new NotImplementedException();
    IEnumerator<int> IEnumerable<int>.GetEnumerator() => throw new NotImplementedException();
    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

    string IReadOnlyList<string>.this[int index] => throw new NotImplementedException();
    int IReadOnlyCollection<string>.Count => throw new NotImplementedException();
    IEnumerator<string> IEnumerable<string>.GetEnumerator() => throw new NotImplementedException();
}
";
            await TestInRegularAndScriptAsync(text, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task MixedInterfaceImplementation()
        {
            var text = @"
using System;
using System.Collections;
using System.Collections.Generic;

class Test
{
    void Method()
    {
        var list = new Mixed();
        foreach [||] (var a in list)
        {
            Console.WriteLine(a);
        }
    }
}

class Mixed : IReadOnlyList<int>, IReadOnlyList<string>
{
    public int this[int index] => throw new NotImplementedException();
    public int Count => throw new NotImplementedException();
    public IEnumerator<int> GetEnumerator() => throw new NotImplementedException();
    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

    string IReadOnlyList<string>.this[int index] => throw new NotImplementedException();
    int IReadOnlyCollection<string>.Count => throw new NotImplementedException();
    IEnumerator<string> IEnumerable<string>.GetEnumerator() => throw new NotImplementedException();
}
";

            var expected = @"
using System;
using System.Collections;
using System.Collections.Generic;

class Test
{
    void Method()
    {
        var list = new Mixed();
        for (var {|Rename:i|} = 0; i < list.Count; i++)
        {
            var a = list[i];
            Console.WriteLine(a);
        }
    }
}

class Mixed : IReadOnlyList<int>, IReadOnlyList<string>
{
    public int this[int index] => throw new NotImplementedException();
    public int Count => throw new NotImplementedException();
    public IEnumerator<int> GetEnumerator() => throw new NotImplementedException();
    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

    string IReadOnlyList<string>.this[int index] => throw new NotImplementedException();
    int IReadOnlyCollection<string>.Count => throw new NotImplementedException();
    IEnumerator<string> IEnumerable<string>.GetEnumerator() => throw new NotImplementedException();
}
";
            await TestInRegularAndScriptAsync(text, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task MixedInterfaceImplementationWithExplicitType()
        {
            var text = @"
using System;
using System.Collections;
using System.Collections.Generic;

class Test
{
    void Method()
    {
        var list = new Mixed();
        foreach [||] (string a in list)
        {
            Console.WriteLine(a);
        }
    }
}

class Mixed : IReadOnlyList<int>, IReadOnlyList<string>
{
    public int this[int index] => throw new NotImplementedException();
    public int Count => throw new NotImplementedException();
    public IEnumerator<int> GetEnumerator() => throw new NotImplementedException();
    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

    string IReadOnlyList<string>.this[int index] => throw new NotImplementedException();
    int IReadOnlyCollection<string>.Count => throw new NotImplementedException();
    IEnumerator<string> IEnumerable<string>.GetEnumerator() => throw new NotImplementedException();
}
";

            var expected = @"
using System;
using System.Collections;
using System.Collections.Generic;

class Test
{
    void Method()
    {
        var list = new Mixed();
        var {|Rename:list1|} = (IReadOnlyList<string>)list;
        for (var {|Rename:i|} = 0; i < list1.Count; i++)
        {
            var a = list1[i];
            Console.WriteLine(a);
        }
    }
}

class Mixed : IReadOnlyList<int>, IReadOnlyList<string>
{
    public int this[int index] => throw new NotImplementedException();
    public int Count => throw new NotImplementedException();
    public IEnumerator<int> GetEnumerator() => throw new NotImplementedException();
    IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

    string IReadOnlyList<string>.this[int index] => throw new NotImplementedException();
    int IReadOnlyCollection<string>.Count => throw new NotImplementedException();
    IEnumerator<string> IEnumerable<string>.GetEnumerator() => throw new NotImplementedException();
}
";
            await TestInRegularAndScriptAsync(text, expected);
        }

        [Fact, Trait(Traits.Feature, Traits.Features.ForeachToFor)]
        public async Task PreserveUserExpression()
        {
            var text = @"
using System;
using System.Collections;
using System.Collections.Generic;

namespace NS
{
    class Test
    {
        void Method()
        {
            foreach [||] (string a in new NS.Mixed())
            {
                Console.WriteLine(a);
            }
        }
    }

    class Mixed : IReadOnlyList<int>, IReadOnlyList<string>
    {
        public int this[int index] => throw new NotImplementedException();
        public int Count => throw new NotImplementedException();
        public IEnumerator<int> GetEnumerator() => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

        string IReadOnlyList<string>.this[int index] => throw new NotImplementedException();
        int IReadOnlyCollection<string>.Count => throw new NotImplementedException();
        IEnumerator<string> IEnumerable<string>.GetEnumerator() => throw new NotImplementedException();
    }
}
";

            var expected = @"
using System;
using System.Collections;
using System.Collections.Generic;

namespace NS
{
    class Test
    {
        void Method()
        {
            var {|Rename:list|} = (IReadOnlyList<string>)new NS.Mixed();
            for (var {|Rename:i|} = 0; i < list.Count; i++)
            {
                var a = list[i];
                Console.WriteLine(a);
            }
        }
    }

    class Mixed : IReadOnlyList<int>, IReadOnlyList<string>
    {
        public int this[int index] => throw new NotImplementedException();
        public int Count => throw new NotImplementedException();
        public IEnumerator<int> GetEnumerator() => throw new NotImplementedException();
        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();

        string IReadOnlyList<string>.this[int index] => throw new NotImplementedException();
        int IReadOnlyCollection<string>.Count => throw new NotImplementedException();
        IEnumerator<string> IEnumerable<string>.GetEnumerator() => throw new NotImplementedException();
    }
}
";
            await TestInRegularAndScriptAsync(text, expected);
        }
    }
}
