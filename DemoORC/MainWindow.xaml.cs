using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace DemoORC
{
   /// <summary>
   /// Interaction logic for MainWindow.xaml
   /// </summary>
   public partial class MainWindow : Window
   {
      public MainWindow()
      {
         InitializeComponent();
      }

      public static readonly string PYTHON_PROGRAM_FILE = "get_local.exe";
      public static readonly string DATA_FILE_NAME = "convert_info.txt";
      public static readonly string SEPARATE_STR = "@#$%^&*";
      static int countImageNotConvert = 0;

      private static void HandlingMessage(string outputMessage)
      {
         if (string.IsNullOrEmpty(outputMessage)) { return; }

         var result = outputMessage.Split(new string[] { SEPARATE_STR }, StringSplitOptions.None);

         if (outputMessage.StartsWith("[Image_Not_Converted]"))
         {
            Logger.logCSV("Image_Not_Converted", result.Skip(1).ToArray());
            countImageNotConvert++;
         }
         else if (outputMessage.StartsWith("[Success]") && result.Length > 4)
         {
            Logger.info(result[1].ToInt32(), result[2].ToInt32(), $"Copy and converted \"{result[3]}\" -> \"{result[4]}\" successfully");
         }
      }

      public static void ToolCopyAndConvert()
      {
         using (var process = new Process())
         {
            process.StartInfo = new ProcessStartInfo()
            {
               FileName = "get_local.exe",
               UseShellExecute = false,
               RedirectStandardOutput = true,
               RedirectStandardError = true,
            };

            process.OutputDataReceived += (sender, args) => HandlingMessage(args.Data);
            process.ErrorDataReceived += (sender, args) => HandlingMessage(args.Data);

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            process.WaitForExit(); //you need this in order to flush the output buffer
         }

         if (countImageNotConvert > 0)
         {
            Logger.log($"Not convert {countImageNotConvert} images (detail in csv log)");
         }

         Logger.info("Tool Copy and convert image done");
      }

   }
}
