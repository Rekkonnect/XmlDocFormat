namespace XmlDocFormat.Core.Tests;

public static partial class FormattingTestCases
{
    // ========================================================================
    public static readonly FormatTestCase BasicSummary = new(
        """
        /// <summary>
        /// This is an example method with a very long summary that should be broken into multiple.
        /// </summary>
        public abstract void ExampleMethod();
        """,
        """
        /// <summary>
        /// This is an example method with a very long summary that should be broken
        /// into multiple.
        /// </summary>
        public abstract void ExampleMethod();
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 80,
        });

    // ========================================================================
    public static readonly FormatTestCase BreakElementPreservation = new(
        """
        /// <summary>
        /// This is an example method with a very long summary that should be broken into multiple.
        /// <br/>
        /// More long lines following, but should not merge with the br element.
        /// This line, however should be merged with the previous line.
        /// </summary>
        public abstract void ExampleMethod();
        """,
        """
        /// <summary>
        /// This is an example method with a very long summary that should be broken
        /// into multiple.
        /// <br/>
        /// More long lines following, but should not merge with the br element. This
        /// line, however should be merged with the previous line.
        /// </summary>
        public abstract void ExampleMethod();
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 80,
        });

    // ========================================================================
    public static readonly FormatTestCase ParagraphElementPreservation = new(
        """
        /// <summary>
        /// Begin summary.
        /// <para>
        /// This is an example method with a very long summary that should be broken into multiple.
        /// </para>
        /// <para>
        /// More paragraphs exist in this element.
        /// Short content lines must be merged.
        /// Not the para elements though.
        /// </para>
        /// And especially not with elements outside the para element.
        /// With more content, come more responsibilities.
        /// </summary>
        public abstract void ExampleMethod();
        """,
        """
        /// <summary>
        /// Begin summary.
        /// <para>
        /// This is an example method with a very long summary that should be broken
        /// into multiple.
        /// </para>
        /// <para>
        /// More paragraphs exist in this element. Short content lines must be merged.
        /// Not the para elements though.
        /// </para>
        /// And especially not with elements outside the para element. With more
        /// content, come more responsibilities.
        /// </summary>
        public abstract void ExampleMethod();
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 80,
        });

    // ========================================================================
    public static readonly FormatTestCase LongCrefExceedingMaxLength = new(
        """
        /// <summary>
        /// <para>
        /// This is an example method with a very long summary that should be broken into multiple.
        /// </para>
        /// <para>
        /// This is another paragraph that contains a line with
        /// a very long cref like this: <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// </para>
        /// </summary>
        public abstract void ExampleMethod();
        """,
        """
        /// <summary>
        /// <para>
        /// This is an example method with a
        /// very long summary that should be
        /// broken into multiple.
        /// </para>
        /// <para>
        /// This is another paragraph that
        /// contains a line with a very long
        /// cref like this:
        /// <see cref="System.Collections.Generic.List{T}.Count"/>.
        /// </para>
        /// </summary>
        public abstract void ExampleMethod();
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 40,
        });

    // ========================================================================
    public static readonly FormatTestCase ListTags = new(
        """
        /// <summary>
        /// <list type="bullet">
        ///     <item>
        ///     an item with very long text inside of it that should be broken down to multiple lines
        ///     </item>
        /// </list>
        /// </summary>
        public abstract void ExampleMethod();
        """,
        """
        /// <summary>
        /// <list type="bullet">
        ///     <item>
        ///     an item with very long text
        ///     inside of it that should be
        ///     broken down to multiple lines
        ///     </item>
        /// </list>
        /// </summary>
        public abstract void ExampleMethod();
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 40,
        });

    // ========================================================================
    public static readonly FormatTestCase Comments = new(
        """
        /// <summary>
        /// Some summary for some method that contains some very helpful documentation.
        /// <!-- some comment that contains very helpful and long information 
        /// which could also be wrapped like so but should remain untouched -->
        /// </summary>
        /// <remarks>
        /// Remarks with <!-- some other very helpful comment that forces wrapping the line --> intermittent comments.
        /// </remarks>
        public abstract void ExampleMethod();
        """,
        """
        /// <summary>
        /// Some summary for some method that
        /// contains some very helpful
        /// documentation.
        /// <!-- some comment that contains very helpful and long information 
        /// which could also be wrapped like so but should remain untouched -->
        /// </summary>
        /// <remarks>
        /// Remarks with
        /// <!-- some other very helpful comment that forces wrapping the line -->
        /// intermittent comments.
        /// </remarks>
        public abstract void ExampleMethod();
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 40,
        });

    // ========================================================================
    public static readonly FormatTestCase LongCodeUnaffected = new(
        """
        /// <summary>
        /// Unaffected documentation.
        /// </summary>
        public static virtual void ExampleMethod() { if (true) { System.Console.WriteLine("Hello, World!"); } }
        """,
        """
        /// <summary>
        /// Unaffected documentation.
        /// </summary>
        public static virtual void ExampleMethod() { if (true) { System.Console.WriteLine("Hello, World!"); } }
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 40,
        });

    // ========================================================================
    public static readonly FormatTestCase InlineSummary = new(
        """
        /// <summary>Inline summary</summary>
        public abstract void ExampleMethod();
        """,
        """
        /// <summary>
        /// Inline summary
        /// </summary>
        public abstract void ExampleMethod();
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 40,
        });

    // ========================================================================
    public static readonly FormatTestCase InlineSummaryAndReturnsInsideClass = new(
        """
        /// <summary>Inline summary</summary>
        /// <remarks>This contains a method with a returns element.</remarks>
        public abstract class Abstract
        {
            /// <summary>Inline summary</summary>
            /// <returns>The true <see langword="void" />.</returns>
            public abstract void ExampleMethod();
        }
        """,
        """
        /// <summary>
        /// Inline summary
        /// </summary>
        /// <remarks>
        /// This contains a method with a
        /// returns element.
        /// </remarks>
        public abstract class Abstract
        {
            /// <summary>
            /// Inline summary
            /// </summary>
            /// <returns>
            /// The true
            /// <see langword="void" />.
            /// </returns>
            public abstract void ExampleMethod();
        }
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 40,
        });

    // ========================================================================
    public static readonly FormatTestCase IndentedSummary = new(
        """
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
    public static readonly FormatTestCase InconsistentlyIndentedSummary = new(
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
    public static readonly FormatTestCase MisalignedSummary = new(
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
    public static readonly FormatTestCase StupidLongArrayMarker1 = new(
        """
        /// <summary>
        /// Extension handler for very high-ranked arrays like <typeparamref name="T"/>[,,,,,,,,,,,,,,,,,,,,,,]
        /// that are somehow legal and presentable in the language.
        /// </summary>
        public static class HighRankedArrayExtensions<T>;
        """,
        """
        /// <summary>
        /// Extension handler for very high-ranked arrays like
        /// <typeparamref name="T"/>[,,,,,,,,,,,,,,,,,,,,,,] that
        /// are somehow legal and presentable in the language.
        /// </summary>
        public static class HighRankedArrayExtensions<T>;
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 60,
        });

    // ========================================================================
    public static readonly FormatTestCase StupidLongArrayMarker2 = new(
        """
        /// <summary>
        /// Supports arrays like <typeparamref name="T"/>[,,,,,,,,,,,,,,,,,,,,,,]
        /// that are somehow legal and presentable in the language.
        /// </summary>
        public static class HighRankedArrayExtensions<T>;
        """,
        """
        /// <summary>
        /// Supports arrays like
        /// <typeparamref name="T"/>[,,,,,,,,,,,,,,,,,,,,,,] that
        /// are somehow legal and presentable in the language.
        /// </summary>
        public static class HighRankedArrayExtensions<T>;
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 60,
        });

    // ========================================================================
    public static readonly FormatTestCase Ellipses = new(
        """
        /// <summary>
        /// This questionable item is a big WIP...
        /// </summary>
        public static class VeryQuestionableItem;
        """,
        """
        /// <summary>
        /// This questionable item is a big
        /// WIP...
        /// </summary>
        public static class VeryQuestionableItem;
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 40,
        });

    // ========================================================================
    public static readonly FormatTestCase DashAfterSeeRef = new(
        """
        /// <summary>
        /// Detects how count-based properties
        /// -such as <see cref="System.Collections.Generic.List{T}.Count"/>-
        /// behave and affect the resilience of collections.
        /// </summary>
        public static class CollectionCountHandler;
        """,
        """
        /// <summary>
        /// Detects how count-based properties
        /// -such as
        /// <see cref="System.Collections.Generic.List{T}.Count"/>-
        /// behave and affect the resilience of
        /// collections.
        /// </summary>
        public static class CollectionCountHandler;
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 40,
        });

    // ========================================================================
    public static readonly FormatTestCase XmlEntities = new(
        """
        /// <summary>
        /// Handles escaping of XML special characters such as
        /// '&amp;', '&lt;', '&gt;', '&quot;', and '&apos'
        /// in documentation comments.
        /// </summary>
        public static class XmlEntities;
        """,
        """
        /// <summary>
        /// Handles escaping of XML special characters such as
        /// '&amp;', '&lt;', '&gt;', '&quot;', and '&apos' in
        /// documentation comments.
        /// </summary>
        public static class XmlEntities;
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 60,
        });

    // ========================================================================
    public static readonly FormatTestCase NonNullWithLangword = new(
        """
        /// <summary>
        /// Handles non-<see langword="null"/> values only.
        /// </summary>
        public static class NonNullHandler;
        """,
        """
        /// <summary>
        /// Handles non-<see langword="null"/> values only.
        /// </summary>
        public static class NonNullHandler;
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 80,
        });

    // ========================================================================
    public static readonly FormatTestCase ParenthesizedSeeRef = new(
        """
        /// <summary>
        /// Considers approaches for handling
        /// heavily obsolete collection types (<see cref="System.Collections.ArrayList"/>,
        /// <see cref="System.Collections.SortedList"/>,
        /// <see cref="System.Collections.Hashtable"/>, etc.).
        /// </summary>
        public static class HeavilyObsoleteCollectionsHandler;
        """,
        """
        /// <summary>
        /// Considers approaches for handling
        /// heavily obsolete collection types
        /// (<see cref="System.Collections.ArrayList"/>,
        /// <see cref="System.Collections.SortedList"/>,
        /// <see cref="System.Collections.Hashtable"/>,
        /// etc.).
        /// </summary>
        public static class HeavilyObsoleteCollectionsHandler;
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 40,
        });

    // ========================================================================
    public static readonly FormatTestCase UntouchedWithInlineEmptyElements = new(
        """
        /// <summary>
        /// Handling <see langword="null"/>, and more.
        /// </summary>
        public static class HandlingNullAndMore;
        """,
        """
        /// <summary>
        /// Handling <see langword="null"/>, and more.
        /// </summary>
        public static class HandlingNullAndMore;
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 80,
        });

    // ========================================================================
    public static readonly FormatTestCase WithSyntaxErrors = new(
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
    public static readonly FormatTestCase WithJapaneseText = new(
        """
        /// <summary>コードに日本語の文字をコミットするのは初めてです。</summary>
        public static class 物体;
        """,
        """
        /// <summary>
        /// コードに日本語の文字をコミットするのは初めてです。
        /// </summary>
        public static class 物体;
        """,
        new XmlDocFormatOptions
        {
            MaxLineLength = 40,
        });

    // ========================================================================
    public static readonly FormatTestCase WithAsteriskDocStyle = new(
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
    public static readonly FormatTestCase MixRegularAndAsteriskDocStyle = new(
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
    public static readonly FormatTestCase AsteriskDocStyle = new(
        """
           /** 
            * <exception cref="NotImplementedException">Unimplemented</exception>
            * <exception cref="NotSupportedException">Unsupported</exception>
            */
        public static class SomeType;
        """,
        """
        /// 
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
    public static readonly FormatTestCase AlignOnMixRegularAndAsteriskDocStyle = new(
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
        /// 
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
}
