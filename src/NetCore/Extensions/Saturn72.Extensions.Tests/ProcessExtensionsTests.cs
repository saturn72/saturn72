#region

using System;
using System.Diagnostics;
using System.Threading;
using NUnit.Framework;
using Saturn72.UnitTesting.Framework;

#endregion

namespace Saturn72.Extensions.Tests
{
    public class ProcessExtensionsTests
    {
        [Fact]
        public void IsRunning_ThrowsExceptionOnNullProcess()
        {
            typeof(NullReferenceException).ShouldBeThrownBy(() => ((Process) null).IsRunning());
           
        }

        [Fact]
        public void IsRunning_ReturnsTrue()
        {
            var proc = Process.GetProcesses()[0];
            proc.IsRunning().ShouldBeTrue();
        }

        [Fact]
        public void IsRunning_ReturnsFalseOnNotRunningProcess()
        {
            new Process().IsRunning().ShouldBeFalse();
            var proc = Process.Start("notepad");
            Thread.Sleep(3000);
            proc.CloseMainWindow();
            Thread.Sleep(3000);
            proc.IsRunning().ShouldBeFalse();
        }

        [Fact]
        public void StartInfoAsString_Throws()
        {
            typeof(NullReferenceException).ShouldBeThrownBy(() => ((Process) null).StartInfoAsString());
        }


        [Fact]
        public void AsString_Throws()
        {
            typeof(NullReferenceException).ShouldBeThrownBy(() => ((ProcessStartInfo) null).AsString());
        }


        [Fact]
        public void StartInfoAsString_ReturnsStringWithValues()
        {
            var psi = new ProcessStartInfo
            {
                WorkingDirectory = @"C:\Program Files (x86)\Notepad++",
                FileName = "notepad++.exe",
                Arguments = @"C:\temp\1.bat"
            };

            var proc = new Process { StartInfo = psi };

            proc.StartInfoAsString().ShouldEqual("File name: notepad++.exe\nArguments: C:\\temp\\1.bat\nWorking directory: C:\\Program Files (x86)\\Notepad++");
        }
        [Fact]
        public void StartInfoAsString_ReturnsStringWithEmptyValues()
        {
            var proc = new Process();

            proc.StartInfoAsString().ShouldEqual("File name: \nArguments: \nWorking directory: ");
        }

        [Fact]
        public void AsString_ReturnsString()
        {
            var psi = new ProcessStartInfo
            {
                WorkingDirectory = @"C:\Program Files (x86)\Notepad++",
                FileName = "notepad++.exe",
                Arguments = @"C:\temp\1.bat"
            };

            psi.AsString().ShouldEqual("File name: notepad++.exe\nArguments: C:\\temp\\1.bat\nWorking directory: C:\\Program Files (x86)\\Notepad++");
        }
    }
}