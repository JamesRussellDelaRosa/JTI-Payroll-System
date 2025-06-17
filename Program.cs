using System; // Added for STAThread if not already present, and ApplicationConfiguration
using PdfSharp.Fonts;
using System.Reflection;

namespace JTI_Payroll_System
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Register font resolver for PdfSharp
            PdfSharp.Fonts.GlobalFontSettings.FontResolver = SimpleFontResolver.Instance;

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            Application.Run(new Form1());
        }
    }

    internal class SimpleFontResolver : IFontResolver
    {
        public static readonly SimpleFontResolver Instance = new SimpleFontResolver();
        private static readonly byte[] LiberationSans = GetFontData();

        public byte[] GetFont(string faceName)
        {
            return LiberationSans;
        }

        public FontResolverInfo ResolveTypeface(string familyName, bool isBold, bool isItalic)
        {
            // Map all requests to the embedded font
            return new FontResolverInfo("LiberationSans#") { }; 
        }

        private static byte[] GetFontData()
        {
            // Embedded LiberationSans-Regular.ttf as resource (add to project if not present)
            var asm = Assembly.GetExecutingAssembly();
            using (var stream = asm.GetManifestResourceStream("JTI_Payroll_System.LiberationSans-Regular.ttf"))
            {
                if (stream == null) throw new InvalidOperationException("Font resource not found. Add LiberationSans-Regular.ttf as an embedded resource.");
                byte[] data = new byte[stream.Length];
                stream.Read(data, 0, data.Length);
                return data;
            }
        }
    }
}