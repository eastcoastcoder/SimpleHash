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
        private SimpleLogger _logger = new SimpleLogger();

        public MainWindow()
        {
            InitializeComponent();

            /*
            FileDatum[] testGridData = new FileDatum[] {
                new FileDatum { FileName = "1", Hash = "123" },
                new FileDatum { FileName = "2", Hash = "123" },
                new FileDatum { FileName = "3", Hash = "123" }
            };
            */

            _logger.AddInfo("Initalized");
            //tmrUpdate.Enabled = true;
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
                pyScope.SetVariable("log", _logger);
                _logger.AddInfo("Python Initialized");
            }
            System.Windows.Threading.DispatcherTimer tmrUpdate = new System.Windows.Threading.DispatcherTimer();
            tmrUpdate.Tick += new EventHandler(tmrUpdate_Tick);
            tmrUpdate.Interval = new TimeSpan(0, 0, 1);
            tmrUpdate.Start();
        }

        private void tmrUpdate_Tick(object sender, EventArgs e)
        {
            List<SimpleLogger.Entry> entries = _logger.GetAll();
            foreach (var entry in entries)
            {
                lstEntries.Items.Insert(0, entry);
            }
        }

        private void CompileSourceAndExecute(String code)
        {
            ScriptSource source = pyEngine.CreateScriptSourceFromString(code, SourceCodeKind.Statements);
            CompiledCode compiled = source.Compile();
            // Executes in the scope of Python
            compiled.Execute(pyScope);
        }

        private void CompileSourceFileAndExecute(String file, List<string> argv)
        {
            ScriptSource source = pyEngine.CreateScriptSourceFromFile(file);
            CompiledCode compiled = source.Compile();
            if (argv.Count > 0)
            {
                pyEngine.GetSysModule().SetVariable("argv", argv);
            }
            // Executes in the scope of Python
            compiled.Execute(pyScope);
            // source.Execute(pyScope);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            pyEngine.ExecuteFile(@"Assets\test.py", pyScope);
            String output = pyScope.GetVariable("output");
            myBlock.Text = output;
        }

        /// <summary>
        /// Insert an entry into the logger when it is 
        /// passed into the python executation of a 
        /// function.
        /// </summary>
        /// <returns></returns>
        private String CreatePythonScript1()
        {
            String result = "";
            string[] lines =
                {
                    @"def DoIt1(logObj):",
                    @"   logObj.AddInfo('Executed in a function call using log object input.')",
                    @"",
                };
            result = String.Join("\r", lines);
            return result;
        }

        /// <summary>
        /// Create some variables in the Python scope.
        /// </summary>
        /// <returns></returns>
        private String CreatePythonScript2()
        {
            String result = "";
            string[] lines =
                {
                    @"pyList = ['apples','bananas','grapes']",
                    @"pyDict = { 'key1':'apple','key2':'banana','key3':'grape'}",
                    @"pyString = 'This is a python string'",
                    @"pyCounter = 10",
                    @"",
                    @"def IncrementCounter():",
                    @"   global pyCounter",
                    @"   pyCounter += 1",
                    @"   log.AddInfo('Increment Counter')",
                    @"",
                };
            result = String.Join("\r", lines);
            return result;
        }


        /// <summary>
        /// Create a class in Python and access from C#.
        /// </summary>
        /// <returns></returns>
        private String CreatePythonScript3()
        {
            String result = "";
            string[] lines =
                {
                    @"class Operation(object):",
                    @"  def Operate(self,value):",
                    @"     log.AddInfo('Operate called with value=%d'%value)",
                    @"     return value*value",
                    @"",
                };
            result = String.Join("\r", lines);
            return result;
        }

        /// <summary>
        /// Define an iterator function (yield) in Python.
        /// </summary>
        /// <returns></returns>
        private String CreatePythonScript4()
        {
            String result = "";
            string[] lines =
                {
                    // Defines an iterable class
                    @"pyIterList = [11,22,33]",
                    @"",
                    @"class Permutation:",
                    @"    def __init__(self, justalist):",
                    @"        self._data = justalist[:]",
                    @"        self._sofar = []",
                    @"    def __iter__(self):",
                    @"        return self.next()",
                    @"    def next(self):",
                    @"        for elem in self._data:",
                    @"            if elem not in self._sofar:",
                    @"                self._sofar.append(elem)",
                    @"                if len(self._sofar) == len(self._data):",
                    @"                    yield self._sofar[:]",
                    @"                else:",
                    @"                    for v in self.next():",
                    @"                        yield v",
                    @"                self._sofar.pop()",
                    @"",
                    // Defines a simple iterable function
                    @"def Yielder(collection):",
                    @"   iterColl = iter(collection)",
                    @"   for item in iterColl:",
                    @"       yield item",
                    @"",

                };
            result = String.Join("\r", lines);
            return result;
        }

  
        /// <summary>
        /// Execute a function with an input argument.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest1_Click(object sender, EventArgs e)
        {
            CompileSourceAndExecute(CreatePythonScript1());
            CompileSourceAndExecute("DoIt1(log)");
        }

        /// <summary>
        /// Execute a series of operations of variables in the Python scope (module level).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest2_Click(object sender, EventArgs e)
        {
            CompileSourceAndExecute(CreatePythonScript2());
            // pyList
            // Display in the Python Side
            CompileSourceAndExecute("for item in pyList: log.AddInfo('List Item: %s'%item)");
            // Read from the C# side
            // NOTE:  I don't have to specify IronPython.Runtime.List, the compiler figures this out.
            // NOTE:  I figured out the type by retrieving it as an "object" first, the debugging.
            List lst = pyScope.GetVariable("pyList") as List;
            List<String> lstTemp = new List<string>();
            for (Int32 idx = 0; idx < lst.Count; idx++)
            {
                _logger.AddInfo(String.Format("Python List in C#: {0}", lst[idx].ToString()));
                lstTemp.Add(lst[idx].ToString() + "_GenList");
            }
            // Now set the list to a generic "list"
            pyScope.SetVariable("pyList", lstTemp);
            CompileSourceAndExecute("for item in pyList: log.AddInfo('List Item: %s'%item)");

            // pyDict
            CompileSourceAndExecute("for key in pyDict.keys(): log.AddInfo('Dict Item: %s'%pyDict[key])");

            // pyString
            CompileSourceAndExecute("log.AddInfo(pyString)");
            // Modify the string in Python
            CompileSourceAndExecute("pyString += '_Mod_By_Python'");
            CompileSourceAndExecute("log.AddInfo(pyString)");
            // Modify the string in C#
            String str = pyScope.GetVariable("pyString") as String;
            str += "_Mod_By_C#";
            // NOTE:  The string is "atomic", so we have to set it back in Python.
            // If we want the "mod" without the "set", wrap it in a Python class (or function) and
            // use an accessor to modify it.  Then you are only manipulating the 
            // reference, not the object (we'll show an example).
            pyScope.SetVariable("pyString", str);
            CompileSourceAndExecute("log.AddInfo(pyString)");

            // pyCounter
            CompileSourceAndExecute("log.AddInfo('Counter = %s'%pyCounter)");
            CompileSourceAndExecute("IncrementCounter()");
            CompileSourceAndExecute("log.AddInfo('Counter = %s'%pyCounter)");
            Int32 counter = (Int32)pyScope.GetVariable("pyCounter");
            _logger.AddInfo(String.Format("Counter++ (from C# side) = {0}", counter++));
            pyScope.SetVariable("pyCounter", counter);
            CompileSourceAndExecute("log.AddInfo('Counter = %s'%pyCounter)");

        }

        /// <summary>
        /// Execute an operation defined in a Pythoh class.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest3_Click(object sender, EventArgs e)
        {
            CompileSourceAndExecute(CreatePythonScript3());

            var instance = pyEngine.Execute("Operation()", pyScope);
            var ops = pyEngine.CreateOperations(pyScope);
            Int32 result = (Int32)ops.InvokeMember(instance, "Operate", new object[] { 7 });
            _logger.AddInfo(String.Format("Operation.Operate(7) = {0}", result));
        }

        private void btnTest4_Click(object sender, EventArgs e)
        {
            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this).GetValueOrDefault())
            {
                Console.WriteLine(dialog.SelectedPath);
            }
        }

        /// <summary>
        /// Run Hash.py
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest5_Click(object sender, EventArgs e)
        {
            // pyEngine.ExecuteFile(@"Assets\Hash.py", pyScope);
            List<String> argv = new List<String>();
            //Do some stuff and fill argv
            argv.Add("Hash.py"); // Dummy for usage statement

            var dialog = new Ookii.Dialogs.Wpf.VistaFolderBrowserDialog();
            if (dialog.ShowDialog(this).GetValueOrDefault())
            {
                argv.Add(dialog.SelectedPath);
            }

            CompileSourceFileAndExecute(@"Assets\Hash.py", argv);
            // List<PythonDictionary> output = ((IList<object>)pyScope.GetVariable("resultDictArr")).Cast<PythonDictionary>().ToList();
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

            // Reading from method return
            // source.Execute(scope); // class object created
            // object classObject = pyScope.GetVariable("MyClass"); // get the class object
            // object instance = op.Invoke(classObject); // create the instance
            // object method = op.GetMember(instance, "main"); // get a method
            // List<string> result = (List<string>)op.Invoke(method);

            /*
            if (output.Count != 0)
            {
                myBlock.Text = output[0];
            } else
            {
                myBlock.Text = "No files found in input directory";
            }
            */
        }

        /// <summary>
        /// Throw an error while executing a script
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTest6_Click(object sender, EventArgs e)
        {
            try
            {
                _logger.AddInfo(">>>> YOU SHOULD SEE AN EXECUTION (div 0) FAULT NEXT <<<<");
                CompileSourceAndExecute("for x in [1,2,3,0]:y = 2/x");
            }
            catch (Exception ex)
            {
                _logger.AddFault(ex);
            }
        }

        public delegate Int32 PyDelegate(Int32 arg);

        private void btnTest7_Click(object sender, EventArgs e)
        {
            // Matches the signature of PyDelegate
            CompileSourceAndExecute("def pyDelegate(val): return val*val");

            PyDelegate pyDel = pyScope.GetVariable<PyDelegate>("pyDelegate");
            _logger.AddInfo(String.Format("C# delegate, implemented in Python, called in C#: {0} => {1}", 9, pyDel(9)));

        }
    }
}
