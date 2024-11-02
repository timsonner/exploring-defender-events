# Exploring Windows Defender Events  

Researching:  
- [x] What are the event paths?  
- [x] Where are these events stored on disk?  
- What information can be gathered about a log?  
EventLogQuery(), EventLogReader(), EventLogSession(), EventLogConfiguration()  
- Can we create an event framework for event CRUD operations?  
- [x] What are the security rules for each path?  

This project is also a good reference for autocomplete for input search...  
AutoCompleteLibrary is the namespace until I change it...  

Compile the dll  
```powershell  
csc /target:library /out:AutoCompleteLibrary.dll AutoCompleteLibrary.cs
```  

Load the Dll method 1 (local)  
```powershell  
$currentDir = Get-Location
$bytes = [IO.File]::ReadAllBytes("$currentDir\AutoCompleteLibrary.dll")
$assembly = [System.Reflection.Assembly]::Load($bytes)
$instance = $assembly.CreateInstance("AutoCompleteLibrary.AutoComplete")

# Retrieve the options list from GetEventLogPaths
$options = $instance.GetEventLogPaths()

# Run the autocomplete functionality
$instance.Run($options)
```  