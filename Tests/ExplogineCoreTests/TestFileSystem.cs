using System;
using System.Collections.Generic;
using System.IO;
using ExplogineCore;
using FluentAssertions;
using Xunit;

namespace ExplogineCoreTests;

public abstract class TestFileSystem
{
    public abstract IFileSystem Temp { get; }

    [Fact]
    public void basic_file_system_stuff()
    {
        Temp.HasFile("foo/hello.txt").Should().BeFalse();
        Temp.CreateFile("foo/hello.txt");
        Temp.AppendToFile("foo/hello.txt", "this is some text");
        Temp.ReadFile("foo/hello.txt").Trim().Should().Be("this is some text");
        Temp.HasFile("foo/hello.txt").Should().BeTrue();
    }

    [Fact]
    public void create_file_multiple_times()
    {
        Temp.CreateFile("foo/hello.txt");
        Temp.WriteToFile("foo/hello.txt", "hello");
        Temp.CreateFile("foo/hello.txt");
        Temp.CreateFile("foo/hello.txt");
        Temp.CreateFile("foo/hello.txt");

        Temp.ReadFile("foo/hello.txt").Trim().Should().Be("hello");
    }

    [Fact]
    public void creating_a_file_does_not_overwrite_it()
    {
        Temp.CreateFile("foo/hello.txt");
        Temp.AppendToFile("foo/hello.txt", "this is some text");
        Temp.CreateFile("foo/hello.txt");
        Temp.ReadFile("foo/hello.txt").Trim().Should().Be("this is some text");
    }

    [Fact]
    public void creating_a_file_overwrites_if_requested()
    {
        Temp.CreateFile("foo/hello.txt");
        Temp.AppendToFile("foo/hello.txt", "this is some text");
        Temp.ReadFile("foo/hello.txt").Trim().Should().Be("this is some text");
        Temp.CreateOrOverwriteFile("foo/hello.txt");
        Temp.ReadFile("foo/hello.txt").Trim().Should().Be("");
    }

    [Fact]
    public void can_get_list_of_files()
    {
        Temp.CreateFile("test1.txt");
        Temp.CreateFile("test2.txt");
        Temp.CreateFile("test3.txt");
        Temp.CreateFile("foo/test4.txt");
        Temp.CreateFile("foo/test5.txt");
        Temp.CreateFile("foo/bar/test6.txt");
        Temp.GetFilesAt(".").Should().Contain(new List<string>
        {
            "test1.txt", "test2.txt", "test3.txt", "foo/test4.txt", "foo/test5.txt",
            "foo/bar/test6.txt"
        });
    }

    [Fact]
    public void can_create_and_write_to_file()
    {
        Temp.WriteToFile("foo/hello.txt", "this is some text");
        Temp.ReadFile("foo/hello.txt").Trim().Should().Be("this is some text");
    }

    [Fact]
    public void can_delete_files()
    {
        Temp.WriteToFile("foo/hello.txt", "this is some text");
        Temp.ReadFile("foo/hello.txt").Trim().Should().Be("this is some text");
        Temp.DeleteFile("foo/hello.txt");
        Temp.ReadFile("foo/hello.txt").Should().Be("");
    }

    public class Real : TestFileSystem, IDisposable
    {
        public Real()
        {
            Temp = new RealFileSystem(Path.Join(Path.GetTempPath(), Guid.NewGuid().ToString()));
        }

        public override IFileSystem Temp { get; }

        public void Dispose()
        {
            if (Temp is RealFileSystem real)
            {
                Directory.Delete(real.RootPath, true);
            }
        }
    }

    public class Virtual : TestFileSystem, IDisposable
    {
        public Virtual()
        {
            Temp = new VirtualFileSystem();
        }

        public override IFileSystem Temp { get; }

        public void Dispose()
        {
            if (Temp is RealFileSystem real)
            {
                Directory.Delete(real.RootPath, true);
            }
        }
    }
}
