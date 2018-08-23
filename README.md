This is a C# and Python app which wraps the result of Python calls in a WPF frontend. 


This was heavily ported from the following project:


https://www.codeproject.com/articles/53611/embedding-ironpython-in-a-c-application


That app was a Windows Forms application which was used as the basis to instead create a Windows Presentation Foundation. Nearly all the business logic was reusable. That project heavily made use of a logger class. In this project, an additional button was added which executes a Python script file instead of an inline Python string. The result of the file is saved to a variable and simply replaces the text in a TextBlock. Additional scripts can be added to the Assets folder in the solution and will be avaliable as long as the file's Copy to Output property is set to Copy Always.


You will need a copy of IronPython installed at C:\Program Files\IronPython 2.7 to get access to StdLib calls


http://ironpython.net/download/