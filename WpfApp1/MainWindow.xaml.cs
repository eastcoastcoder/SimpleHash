using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using IronPython.Hosting;
using IronPython.Runtime;
using Microsoft.Scripting;
using Microsoft.Scripting.Hosting;

namespace WpfApp1
{

    public class FileDatum
    {
        public string FileName { get; set; }
        public string Hash { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ScriptEngine pyEngine = null;
        private ScriptRuntime pyRuntime = null;
        private ScriptScope pyScope = null;
        private ObjectOperations op;

        public MainWindow()
        {
            InitializeComponent();
            if (pyEngine == null)
            {
                ScriptRuntimeSetup setup = Python.CreateRuntimeSetup(null);
                pyRuntime = new ScriptRuntime(setup);
                pyEngine = Python.GetEngine(pyRuntime);
                op = pyEngine.Operations;

                string dir = Path.GetDirectoryName(Environment.CurrentDirectory);
                string assetsDir = Environment.CurrentDirectory + "\\Assets";
                ICollection<string> paths = pyEngine.GetSearchPaths();

                if (!String.IsNullOrWhiteSpace(dir))
                {
                    paths.Add(dir);
                }
                else
                {
                    paths.Add(Environment.CurrentDirectory);
                }
                paths.Add(assetsDir);
                paths.Add("C:\\Program Files\\IronPython 2.7\\Lib");

                pyEngine.SetSearchPaths(paths);
                pyScope = pyEngine.CreateScope();
            }
        }

        private void CompileSourceFileAndExecute(String file, List<string> argv)
        {
            ScriptSource source = pyEngine.CreateScriptSourceFromFile(file);
            CompiledCode compiled = source.Compile();
            if (argv.Count > 0)
            {
                pyEngine.GetSysModule().SetVariable("argv", argv);
            }
            compiled.Execute(pyScope);
        }

        /// <summary>
        /// Run Hash.py
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest5_Click(object sender, EventArgs e)
        {
            List<String> argv = new List<String>();
            argv.Add("Hash.py"); // Dummy for usage statement

            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this).GetValueOrDefault())
            {
                argv.Add(dialog.SelectedPath);
            }

            CompileSourceFileAndExecute(@"Assets\Hash.py", argv);
            dynamic main = pyScope.GetVariable("main");
            List<PythonDictionary> output = ((IList<object>)main()).Cast<PythonDictionary>().ToList();


            List<FileDatum> testGridDataList = new List<FileDatum>();

            output.ForEach((i) => {
                string curFileName = i.get("FileName").ToString();
                string curHash = i.get("Hash").ToString();
                testGridDataList.Add(new FileDatum { FileName = curFileName, Hash = curHash });
            });
            FileDatum[] testGridData = testGridDataList.ToArray();
            myDataGrid.ItemsSource = testGridData;
        }
    }
}
