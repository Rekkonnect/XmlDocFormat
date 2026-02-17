using XmlDocFormat.Core;

namespace XmlDocFormat.Tests.Shared;

public static partial class VisualBasicFormatTestCases
{
    // ========================================================================
    public static readonly VisualBasicFormatTestCase InconsistentlyIndentedSummary = new(
        """
          ''' <summary>Outer container</summary>
        Public NotInheritable Class OuterContainer

           ''' <summary>Example Method</summary>
           Public Shared Sub ExampleMethod()
           End Sub

             ''' <summary>Inner container</summary>
             Public NotInheritable Class InnerContainer

                   ''' <summary>Example Method Inner</summary>
                   Public Shared Sub ExampleMethodInner()
                   End Sub
        
             End Class

        End Class
        """,
        """
        ''' <summary>
        ''' Outer container
        ''' </summary>
        Public NotInheritable Class OuterContainer

           ''' <summary>
           ''' Example Method
           ''' </summary>
           Public Shared Sub ExampleMethod()
           End Sub

             ''' <summary>
             ''' Inner container
             ''' </summary>
             Public NotInheritable Class InnerContainer

                   ''' <summary>
                   ''' Example Method Inner
                   ''' </summary>
                   Public Shared Sub ExampleMethodInner()
                   End Sub

             End Class

        End Class
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 40,
        });

    // ========================================================================
    public static readonly VisualBasicFormatTestCase MisalignedSummary = new(
        """
          ''' <summary>Outer container</summary>
        Public NotInheritable Class OuterContainer

           ''' <summary>Example Method</summary>
            Public Shared Sub ExampleMethod()
            End Sub
        
             ''' <summary>Example Method</summary>
            Public Shared Sub ExampleMethod2()
            End Sub

        End Class
        """,
        """
        ''' <summary>
        ''' Outer container
        ''' </summary>
        Public NotInheritable Class OuterContainer

            ''' <summary>
            ''' Example Method
            ''' </summary>
            Public Shared Sub ExampleMethod()
            End Sub

            ''' <summary>
            ''' Example Method
            ''' </summary>
            Public Shared Sub ExampleMethod2()
            End Sub

        End Class
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 40,
        });

    // ========================================================================
    public static readonly VisualBasicFormatTestCase WithSyntaxErrors = new(
        """
        ''' <summary>Invalid class</summary>
        Public NotInheritable Class
        End Class

        ''' <summary>Another invalid class</summary>
        NotInheritable Class
        End Class
        
        ''' <summary>Another invalid class 2</summary>
        NotInheritable Class
        End Class
        """,
        """
        ''' <summary>
        ''' Invalid class
        ''' </summary>
        Public NotInheritable Class
        End Class
        
        ''' <summary>
        ''' Another invalid class
        ''' </summary>
        NotInheritable Class
        End Class
        
        ''' <summary>
        ''' Another invalid class 2
        ''' </summary>
        NotInheritable Class
        End Class
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 40,
        });

    // ========================================================================
    public static readonly VisualBasicFormatTestCase BasicSummary = new(
        """
        ''' <summary>
        ''' This is an example method with a very long summary that should be broken into multiple.
        ''' </summary>
        Public Overridable Sub ExampleMethod()
        """,
        """
        ''' <summary>
        ''' This is an example method with a very long summary that should be broken
        ''' into multiple.
        ''' </summary>
        Public Overridable Sub ExampleMethod()
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 80,
        });

    // ========================================================================
    // cref is not being formatted currently, but should be handled in the future
    public static readonly VisualBasicFormatTestCase WithUnformattedGenericCref = new(
        """
        ''' <summary>
        ''' Contains the <see cref="Generic (Of T ) . GenericMethod (Of U) (U, ByRef T )"/> method.
        ''' </summary>
        Public NotInheritable Class Generic(Of T)

            Public Shared Sub GenericMethod(Of U)(a As U, ByRef b As T)
            End Sub

        End Class
        """,
        """
        ''' <summary>
        ''' Contains the
        ''' <see cref="Generic (Of T ) . GenericMethod (Of U) (U, ByRef T )"/>
        ''' method.
        ''' </summary>
        Public NotInheritable Class Generic(Of T)

            Public Shared Sub GenericMethod(Of U)(a As U, ByRef b As T)
            End Sub

        End Class
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 40,
        });

    // ========================================================================
    // The formatting only works on the enabled branch
    // This is a quite difficult problem to solve, which can be revisited and
    // fixed only if the experience of applying such changes is not too convoluted
    public static readonly VisualBasicFormatTestCase WithIfDirective = new(
        """
        #If VALUE
        ''' <summary>Basic class</summary>
        #Else
        ''' <summary>Basic class but without value</summary>
        #End If
        Public NotInheritable Class SomeClass
        End Class
        """,
        """
        #If VALUE
        ''' <summary>Basic class</summary>
        #Else
        ''' <summary>
        ''' Basic class but without value
        ''' </summary>
        #End If
        Public NotInheritable Class SomeClass
        End Class
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 40,
        });

    // ========================================================================
    public static readonly VisualBasicFormatTestCase ManyXmlTokenKinds = new(
        """
        ''' <help:meta lang="vb" name="Generic"/>
        ''' <summary>
        ''' A long summary of text that properly documents the purpose of a generic (&gt;T&lt;) type
        ''' which will be expanded in the future to support more complex features and utilities,
        ''' as it is the backbone of this library. For reference, there are the following classes:
        ''' <list type="bullet">
        '''     <item>
        '''     <seealso cref="AnotherGeneric(Of T)">Another generic 1</seealso>
        '''     </item>
        '''     <?generic lang="vb"?>
        '''     <item>
        '''     <seealso cref="AnotherGeneric2(Of T)">Another generic 2</seealso>
        '''     </item>
        '''     <?generic lang="cs"?>
        '''     <item>
        '''     <seealso cref="AnotherGeneric3(Of T)">Another generic 3</seealso>
        '''     </item>
        '''     <?help:end lang="vb"?>
        ''' </list>
        ''' </summary>
        ''' <remarks>
        ''' The referenced alternatives are not exhaustive, and the user could for example expand the
        ''' possible types that are referrable by allowing any of the following scenarios:
        ''' <list type="bullet">
        '''     <item>
        '''     <para>
        '''     Ensure the user first has the ability to declare those classes in their assembly.
        '''     </para>
        '''     <para>
        '''     Create the class of interest and make it referrable by using the following documentation:
        '''     <br/>
        '''     <!-- TODO: Expand the example to support more cases -->
        '''     <![CDATA[<?generic lang="vb"?><seealso cref="(user class)"/>]]>
        '''     </para>
        '''     </item>
        ''' </list>
        ''' It's important to account for the name of the generic type parameter, which should always
        ''' match that of <typeparamref name="T"/>, as it's being referred to.
        ''' </remarks>
        Public NotInheritable Class Generic(Of T)
        End Class
        """,
        """
        ''' <help:meta lang="vb" name="Generic"/>
        ''' <summary>
        ''' A long summary of text that properly documents the purpose of a generic
        ''' (&gt;T&lt;) type which will be expanded in the future to support more
        ''' complex features and utilities, as it is the backbone of this library. For
        ''' reference, there are the following classes:
        ''' <list type="bullet">
        '''     <item>
        '''     <seealso cref="AnotherGeneric(Of T)">Another generic 1</seealso>
        '''     </item>
        '''     <?generic lang="vb"?>
        '''     <item>
        '''     <seealso cref="AnotherGeneric2(Of T)">Another generic 2</seealso>
        '''     </item>
        '''     <?generic lang="cs"?>
        '''     <item>
        '''     <seealso cref="AnotherGeneric3(Of T)">Another generic 3</seealso>
        '''     </item>
        '''     <?help:end lang="vb"?>
        ''' </list>
        ''' </summary>
        ''' <remarks>
        ''' The referenced alternatives are not exhaustive, and the user could for
        ''' example expand the possible types that are referrable by allowing any of the
        ''' following scenarios:
        ''' <list type="bullet">
        '''     <item>
        '''     <para>
        '''     Ensure the user first has the ability to declare those classes in their
        '''     assembly.
        '''     </para>
        '''     <para>
        '''     Create the class of interest and make it referrable by using the
        '''     following documentation:
        '''     <br/>
        '''     <!-- TODO: Expand the example to support more cases -->
        '''     <![CDATA[<?generic lang="vb"?><seealso cref="(user class)"/>]]>
        '''     </para>
        '''     </item>
        ''' </list>
        ''' It's important to account for the name of the generic type parameter, which
        ''' should always match that of <typeparamref name="T"/>, as it's being referred
        ''' to.
        ''' </remarks>
        Public NotInheritable Class Generic(Of T)
        End Class
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 80,
        });
}
