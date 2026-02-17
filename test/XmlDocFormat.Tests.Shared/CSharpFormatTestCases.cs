using XmlDocFormat.Core;

namespace XmlDocFormat.Tests.Shared;

public static partial class CSharpFormatTestCases
{
    // ========================================================================
    public static readonly CSharpFormatTestCase InconsistentlyIndentedSummary = new(
        """
          /// <summary>Outer container</summary>
        public static class OuterContainer
        {
           /// <summary>Example Method</summary>
           public static void ExampleMethod() { }

             /// <summary>Inner container</summary>
             public static class InnerContainer
             {
                   /// <summary>Example Method Inner</summary>
                   public static void ExampleMethodInner();
             }
        }
        """,
        """
        /// <summary>
        /// Outer container
        /// </summary>
        public static class OuterContainer
        {
           /// <summary>
           /// Example Method
           /// </summary>
           public static void ExampleMethod() { }

             /// <summary>
             /// Inner container
             /// </summary>
             public static class InnerContainer
             {
                   /// <summary>
                   /// Example Method Inner
                   /// </summary>
                   public static void ExampleMethodInner();
             }
        }
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 40,
        });

    // ========================================================================
    public static readonly CSharpFormatTestCase MisalignedSummary = new(
        """
          /// <summary>Outer container</summary>
        public static class OuterContainer
        {
           /// <summary>Example Method</summary>
            public static void ExampleMethod() { }
        
             /// <summary>Example Method</summary>
            public static void ExampleMethod2() { }
        }
        """,
        """
        /// <summary>
        /// Outer container
        /// </summary>
        public static class OuterContainer
        {
            /// <summary>
            /// Example Method
            /// </summary>
            public static void ExampleMethod() { }

            /// <summary>
            /// Example Method
            /// </summary>
            public static void ExampleMethod2() { }
        }
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 40,
        });

    // ========================================================================
    // cref is not being formatted currently, but should be handled in the future
    public static readonly CSharpFormatTestCase WithUnformattedGenericCref = new(
        """
        /// <summary>
        /// Contains the <see cref="Generic{T}.GenericMethod { U } (U, out  T )"/> method.
        /// </summary>
        public static class Generic<T>
        {
            public static void GenericMethod<U>(U a, out T b);
        }
        """,
        """
        /// <summary>
        /// Contains the
        /// <see cref="Generic{T}.GenericMethod { U } (U, out  T )"/>
        /// method.
        /// </summary>
        public static class Generic<T>
        {
            public static void GenericMethod<U>(U a, out T b);
        }
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 40,
        });

    // ========================================================================
    public static readonly CSharpFormatTestCase WithSyntaxErrors = new(
        """
        /// <summary>Invalid class</summary>
        public static class;

        /// <summary>Another invalid class</summary>
        static class
        
        /// <summary>Another invalid class 2</summary>
        static class
        """,
        """
        /// <summary>
        /// Invalid class
        /// </summary>
        public static class;
        
        /// <summary>
        /// Another invalid class
        /// </summary>
        static class
        
        /// <summary>
        /// Another invalid class 2
        /// </summary>
        static class
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 40,
        });

    // ========================================================================
    public static readonly CSharpFormatTestCase WithAsteriskDocStyle = new(
        """
        /** <summary>This is now more complex</summary>
         */
        public static class SomeType;
        """,
        """
        /// <summary>
        /// This is now more complex
        /// </summary>
        public static class SomeType;
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 40,
        });

    // ========================================================================
    public static readonly CSharpFormatTestCase MixRegularAndAsteriskDocStyle = new(
        """
        /** <summary>This is now more complex</summary>
         */
        /// <remarks>With remarks</remarks>
        public static class SomeType;
        """,
        """
        /// <summary>
        /// This is now more complex
        /// </summary>
        /// <remarks>
        /// With remarks
        /// </remarks>
        public static class SomeType;
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 40,
        });

    // ========================================================================
    public static readonly CSharpFormatTestCase AsteriskDocStyle = new(
        """
           /** 
            * <exception cref="NotImplementedException">Unimplemented</exception>
            * <exception cref="NotSupportedException">Unsupported</exception>
            */
        public static class SomeType;
        """,
        """
        /// <exception cref="NotImplementedException">
        /// Unimplemented
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Unsupported
        /// </exception>
        public static class SomeType;
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 40,
        });

    // ========================================================================
    public static readonly CSharpFormatTestCase AlignOnMixRegularAndAsteriskDocStyle = new(
        """
           /** <summary>This is now more complex</summary>
            */
           /// <remarks>With remarks</remarks>
           /** 
            * <exception cref="NotImplementedException">Unimplemented</exception>
            * <exception cref="NotSupportedException">Unsupported</exception>
            */
        public static class SomeType;
        """,
        """
        /// <summary>
        /// This is now more complex
        /// </summary>
        /// <remarks>
        /// With remarks
        /// </remarks>
        /// <exception cref="NotImplementedException">
        /// Unimplemented
        /// </exception>
        /// <exception cref="NotSupportedException">
        /// Unsupported
        /// </exception>
        public static class SomeType;
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 40,
        });

    // ========================================================================
    // The formatting only works on the enabled branch
    // This is a quite difficult problem to solve, which can be revisited and
    // fixed only if the experience of applying such changes is not too convoluted
    public static readonly CSharpFormatTestCase WithIfDirective = new(
        """
        #if VALUE
        /// <summary>Basic class</summary>
        #else
        /// <summary>Basic class but without value</summary>
        #endif
        public sealed class SomeClass;
        """,
        """
        #if VALUE
        /// <summary>Basic class</summary>
        #else
        /// <summary>
        /// Basic class but without value
        /// </summary>
        #endif
        public sealed class SomeClass;
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 40,
        });

    // ========================================================================
    public static readonly CSharpFormatTestCase ManyXmlTokenKinds = new(
        """
        /// <help:meta lang="cs" name="Generic"/>
        /// <summary>
        /// A long summary of text that properly documents the purpose of a generic (&gt;T&lt;) type
        /// which will be expanded in the future to support more complex features and utilities,
        /// as it is the backbone of this library. For reference, there are the following classes:
        /// <list type="bullet">
        ///     <item>
        ///     <seealso cref="AnotherGeneric{T}">Another generic 1</seealso>
        ///     </item>
        ///     <?generic lang="vb"?>
        ///     <item>
        ///     <seealso cref="AnotherGeneric2{T}">Another generic 2</seealso>
        ///     </item>
        ///     <?generic lang="cs"?>
        ///     <item>
        ///     <seealso cref="AnotherGeneric3{T}">Another generic 3</seealso>
        ///     </item>
        ///     <?help:end lang="vb"?>
        /// </list>
        /// </summary>
        /// <remarks>
        /// The referenced alternatives are not exhaustive, and the user could for example expand the
        /// possible types that are referrable by allowing any of the following scenarios:
        /// <list type="bullet">
        ///     <item>
        ///     <para>
        ///     Ensure the user first has the ability to declare those classes in their assembly.
        ///     </para>
        ///     <para>
        ///     Create the class of interest and make it referrable by using the following documentation:
        ///     <br/>
        ///     <!-- TODO: Expand the example to support more cases -->
        ///     <![CDATA[<?generic lang="cs"?><seealso cref="(user class)"/>]]>
        ///     </para>
        ///     </item>
        /// </list>
        /// It's important to account for the name of the generic type parameter, which should always
        /// match that of <typeparamref name="T"/>, as it's being referred to.
        /// </remarks>
        public static class Generic<T>;
        """,
        """
        /// <help:meta lang="cs" name="Generic"/>
        /// <summary>
        /// A long summary of text that properly documents the purpose of a generic
        /// (&gt;T&lt;) type which will be expanded in the future to support more
        /// complex features and utilities, as it is the backbone of this library. For
        /// reference, there are the following classes:
        /// <list type="bullet">
        ///     <item>
        ///     <seealso cref="AnotherGeneric{T}">Another generic 1</seealso>
        ///     </item>
        ///     <?generic lang="vb"?>
        ///     <item>
        ///     <seealso cref="AnotherGeneric2{T}">Another generic 2</seealso>
        ///     </item>
        ///     <?generic lang="cs"?>
        ///     <item>
        ///     <seealso cref="AnotherGeneric3{T}">Another generic 3</seealso>
        ///     </item>
        ///     <?help:end lang="vb"?>
        /// </list>
        /// </summary>
        /// <remarks>
        /// The referenced alternatives are not exhaustive, and the user could for
        /// example expand the possible types that are referrable by allowing any of the
        /// following scenarios:
        /// <list type="bullet">
        ///     <item>
        ///     <para>
        ///     Ensure the user first has the ability to declare those classes in their
        ///     assembly.
        ///     </para>
        ///     <para>
        ///     Create the class of interest and make it referrable by using the
        ///     following documentation:
        ///     <br/>
        ///     <!-- TODO: Expand the example to support more cases -->
        ///     <![CDATA[<?generic lang="cs"?><seealso cref="(user class)"/>]]>
        ///     </para>
        ///     </item>
        /// </list>
        /// It's important to account for the name of the generic type parameter, which
        /// should always match that of <typeparamref name="T"/>, as it's being referred
        /// to.
        /// </remarks>
        public static class Generic<T>;
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 80,
        });
}
