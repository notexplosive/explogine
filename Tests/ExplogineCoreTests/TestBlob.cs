using System;
using ApprovalTests;
using ExplogineCore;
using FluentAssertions;
using Xunit;

namespace ExplogineCoreTests;

public class TestBlob
{
    public IFileSystem FileSystem { get; } = new VirtualFileSystem();

    [Fact]
    public void in_memory_primitive_blob()
    {
        var blob = new SerialBlob();

        var floatA = blob.Declare<float>("float_variable_a");

        blob.Set(floatA, 3.1415f);
        blob.Get(floatA).Should().Be(3.1415f);
    }

    [Fact]
    public void fail_to_declare_same_name_twice()
    {
        var blob = new SerialBlob();
        blob.Declare<float>("float_variable_a");

        var lambda = () => { blob.Declare<float>("float_variable_a"); };
        lambda.Should().Throw<Exception>();
    }

    [Fact]
    public void fail_to_retrieve_not_defined_values()
    {
        var blob = new SerialBlob();

        var floatA = blob.Declare<float>("float_variable_a");

        var lambda = () => { blob.Get(floatA); };
        lambda.Should().Throw<Exception>();
    }

    [Fact]
    public void fail_to_retrieve_not_declared_value()
    {
        var blobA = new SerialBlob();
        var blobB = new SerialBlob();

        var floatA = blobB.Declare<float>("float_variable_a");

        var lambda = () => { blobA.GetOrDefault(floatA).Should().Be(0); };
        lambda.Should().Throw<Exception>();
    }

    [Fact]
    public void retrieve_default_when_asked()
    {
        var blob = new SerialBlob();

        var floatA = blob.Declare<float>("float_variable_a");

        blob.GetOrDefault(floatA).Should().Be(0);
    }

    [Fact]
    public void dump_to_file()
    {
        var blob = new SerialBlob();

        blob.Declare("float_variable_a", 1f);
        blob.Declare("float_variable_b", 2f);
        blob.Declare("bool_variable_a", false);
        blob.Declare("enum_variable", TestEnum.TestEnumValueB);
        blob.Declare<bool>("blank_bool_variable");
        blob.Declare<int>("blank_int_variable");

        blob.Dump(FileSystem, "blob.txt");
        var file = FileSystem.ReadFile("blob.txt");
        Approvals.Verify(file);
    }

    [Fact]
    public void write_and_get_back_result()
    {
        // Set some data and write it
        var writeBlob = new SerialBlob();
        writeBlob.Declare("float_variable_a", 1f);
        writeBlob.Declare("float_variable_b", 2f);
        writeBlob.Declare("enum_variable", TestEnum.TestEnumValueC);
        writeBlob.Dump(FileSystem, "written.txt");

        // Another blob (maybe the same blob after restarting the application), only has a subset of the original declared variables.
        var readBlob = new SerialBlob();
        var floatARead = readBlob.Declare<float>("float_variable_a");
        var floatBRead = readBlob.Declare<float>("float_variable_b");
        var enumRead = readBlob.Declare<TestEnum>("enum_variable");
        readBlob.Read(FileSystem, "written.txt");

        readBlob.Get(floatARead).Should().Be(1f);
        readBlob.Get(floatBRead).Should().Be(2f);
        readBlob.Get(enumRead).Should().Be(TestEnum.TestEnumValueC);
    }

    [Fact]
    public void write_and_read_back()
    {
        // Set some data and write it
        var writeBlob = new SerialBlob();
        var floatAWrite = writeBlob.Declare("float_variable_a", 1f);
        var floatBWrite = writeBlob.Declare("float_variable_b", 2f);
        writeBlob.Declare("bool_variable_a", false);
        writeBlob.Dump(FileSystem, "written.txt");

        // Another blob (maybe the same blob after restarting the application), only has a subset of the original declared variables.
        var readBlob = new SerialBlob();
        var floatARead = readBlob.Declare<float>("float_variable_a");
        var floatBRead = readBlob.Declare<float>("float_variable_b");
        readBlob.Read(FileSystem, "written.txt");

        readBlob.Get(floatARead).Should().Be(writeBlob.Get(floatAWrite));
        readBlob.Get(floatBRead).Should().Be(writeBlob.Get(floatBWrite));
    }

    private enum TestEnum
    {
        TestEnumValueA,
        TestEnumValueB,
        TestEnumValueC
    }
}
