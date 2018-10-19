using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.Serialization;
using System.IO;
using System.Xml;
using System.Runtime.Serialization.Formatters.Binary;
using System.Xml.Serialization;

class SerializationComparision
{
    // SerializationComparision is a simple benchmark program that creates a complex object
    // and serializes and deserializes it using various technologies to compare their perf.  
    static void Main()
    {
        // Create a moderately complex, and reasonably object tree to serialize. 
        SerializableClass aSerializableClass = new SerializableClass(40000);

        // Try to serialize and deserialze the object graph using different .NET technologies
        MeasureBinarySerialization(aSerializableClass);
        MeasureDataContractXml(aSerializableClass);
        MeasureDataContractBinary(aSerializableClass);
        MeasureXmlSerialization(aSerializableClass);            // Cheating, Dictionary not serialized
        Console.WriteLine("Warning, XML serialization does not serialize table, can't really be compared to the others.");
    }
    static private SerializableClass MeasureDataContractXml(SerializableClass aSerializableClass)
    {
        Console.WriteLine("Measure System.Runtime.Serialization.DataContractSerializer writing as XML Text");
        DataContractSerializer dcs = new DataContractSerializer(typeof(SerializableClass));
        string dataFileName = "DataContractSerializationData.xml";
        FileStream fs = new FileStream(dataFileName, FileMode.Create);
        using (XmlDictionaryWriter xdw = XmlDictionaryWriter.CreateTextWriter(fs, Encoding.UTF8))
        {
            dcs.WriteObject(xdw, aSerializableClass);
        }

        SerializableClass resultClass;
        fs = new FileStream(dataFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        using (XmlDictionaryReader reader = XmlDictionaryReader.CreateTextReader(fs, new XmlDictionaryReaderQuotas()))
        {
            resultClass = (SerializableClass)dcs.ReadObject(reader);
        }
        fs.Close();
        File.Delete(dataFileName);
        return resultClass;
    }
    static private SerializableClass MeasureDataContractBinary(SerializableClass aSerializableClass)
    {
        Console.WriteLine("Measure System.Runtime.Serialization.DataContractSerializer writing as Binary");
        DataContractSerializer dcs = new DataContractSerializer(typeof(SerializableClass));
        string dataFileName = "DataContractSerializationData.bin";
        FileStream fs = new FileStream(dataFileName, FileMode.Create);
        using (XmlDictionaryWriter xdw = XmlDictionaryWriter.CreateBinaryWriter(fs))
        {
            dcs.WriteObject(xdw, aSerializableClass);
        }

        SerializableClass resultClass;
        fs = new FileStream(dataFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
        using (XmlDictionaryReader reader = XmlDictionaryReader.CreateBinaryReader(fs, new XmlDictionaryReaderQuotas()))
        {
            resultClass = (SerializableClass)dcs.ReadObject(reader);
        }
        fs.Close();
        File.Delete(dataFileName);
        return resultClass;
    }
    static private SerializableClass MeasureBinarySerialization(SerializableClass aSerializableClass)
    {
        Console.WriteLine("Measuring System.Runtime.Serialization.Formatters.Binary.BinaryFormatter Serialization");
        IFormatter formatter = new BinaryFormatter();
        string dataFileName = "BinarySerializationData.bin";
        using (FileStream fs = new FileStream(dataFileName, FileMode.Create))
        {
            CallSerializer(formatter, fs, aSerializableClass);
        }

        SerializableClass resultClass;
        using (FileStream fs = new FileStream(dataFileName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            resultClass = (SerializableClass)formatter.Deserialize(fs);
        }
        File.Delete(dataFileName);
        return resultClass;
    }
    // Helper because it looses a frame
    [System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
    static private void CallSerializer(IFormatter  formatter, FileStream fs, SerializableClass aSerializableClass)
    {
        formatter.Serialize(fs, aSerializableClass);
    }


    static private SerializableClass MeasureXmlSerialization(SerializableClass aSerializableClass)
    {
        Console.WriteLine("Measuring System.Xml.Serialization.XmlSerializer Serialization");
        XmlSerializer mySerializer = new XmlSerializer(typeof(SerializableClass));
        string dataFileName = "XmlSerializationData.xml";
        using (StreamWriter myWriter = new StreamWriter(dataFileName))
        {
        mySerializer.Serialize(myWriter, aSerializableClass);
        }

        SerializableClass resultClass;
        using (FileStream myFileStream = new FileStream(dataFileName, FileMode.Open))
        {
            resultClass = (SerializableClass)mySerializer.Deserialize(myFileStream);
        }
        File.Delete(dataFileName);
        return resultClass;
    }
}

/// <summary>
/// SerializableClass is simply a class with a bunch of members to serialize.  It is recursive 
/// and has a int->string table as part of it.
/// </summary>
[KnownType(typeof(AnotherClass))]
[DataContract]
[Serializable]
[XmlInclude(typeof(AnotherClass))]
public class SerializableClass
{
    public SerializableClass() { myTable = new Dictionary<int, string>(); }
    // Create a complex tree of objects.  It is linear in size parameter  
    public SerializableClass(int size)
    {
        aInstanceInt = 10;
        aInstanceString = "Hello There";

        if (size < 10)
        {
            aMembers = new AnotherClass[size];
            for (int i = 0; i < aMembers.Length; i++)
                aMembers[i] = new AnotherClass(1);
        }
        else
        {
            aMembers = new AnotherClass[0];
            myLeft = new SerializableClass(size / 2);
            myRight = new SerializableClass(size / 2);
        }
        myTable = new Dictionary<int, string>();
        myTable.Add(1, "One");
        myTable.Add(2, "Two");
        myTable.Add(3, "Three");
        myObject = DateTime.Now;
    }

    [DataMember]
    public object myObject;
    [DataMember]
    public SerializableClass myLeft;
    [DataMember]
    public SerializableClass myRight;

    [DataMember]
    [XmlIgnore]
    public Dictionary<int, string> myTable;
    [DataMember]
    public AnotherClass[] aMembers;
    [DataMember]
    public int aInstanceInt;
    [DataMember]
    public string aInstanceString;

    [XmlIgnore]
    public string aNonSerializedString;
}

/// <summary>
/// AnotherClass is simply a class that is used by SerializableClass. 
/// </summary>
[DataContract]
[Serializable]
public class AnotherClass
{
    public AnotherClass(int i)
    {
        aInstanceInt = 50 + i;
        aInstanceInt1 = 51;
        aInstanceInt2 = 52;
        aInstanceInt3 = 53;
        aInstanceInt4 = 54;
        aInstanceInt5 = 55;
        aInstanceInt6 = 56;
        aInstanceInt7 = 57;
        aInstanceString = "AnotherClass";
    }
    public AnotherClass()
    {
    }
    public bool StructuralEqual(object obj)
    {
        var asAnotherClass = obj as AnotherClass;
        if (asAnotherClass == null)
            return false;

        if (aInstanceInt != asAnotherClass.aInstanceInt)
            return false;
        if (aInstanceInt1 != asAnotherClass.aInstanceInt1)
            return false;
        if (aInstanceInt2 != asAnotherClass.aInstanceInt2)
            return false;
        if (aInstanceInt3 != asAnotherClass.aInstanceInt3)
            return false;
        if (aInstanceInt4 != asAnotherClass.aInstanceInt4)
            return false;
        if (aInstanceInt5 != asAnotherClass.aInstanceInt5)
            return false;
        if (aInstanceInt6 != asAnotherClass.aInstanceInt6)
            return false;
        if (aInstanceInt7 != asAnotherClass.aInstanceInt7)
            return false;
        return true;
    }

    [DataMember]
    public int aInstanceInt1;
    [DataMember]
    public int aInstanceInt2;
    [DataMember]
    public int aInstanceInt3;
    [DataMember]
    public int aInstanceInt4;
    [DataMember]
    public int aInstanceInt5;
    [DataMember]
    public int aInstanceInt6;
    [DataMember]
    public int aInstanceInt7;
    [DataMember]
    public int aInstanceInt;
    [DataMember]
    private string aInstanceString;
    [DataMember]
    public SerializableClass loop;
}
