
namespace MongoDB.Profiler
{
    using System;
    using System.ComponentModel;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    // Supported in: 2.6 ... 3.4
    public partial class HostInfo : ISupportInitialize
    {

        [BsonElement("system")] // Supported in: 2.6.12, 3.0.14, 3.0.9, 3.2.12, 3.4.2
        public System_of_HostInfo System { get; set; }

        [BsonElement("os")] // Supported in: 2.6.12, 3.0.14, 3.0.9, 3.2.12, 3.4.2
        public Os_of_HostInfo Os { get; set; }

        [BsonElement("extra")] // Supported in: 2.6.12, 3.0.14, 3.0.9, 3.2.12, 3.4.2
        public Extra_of_HostInfo Extra { get; set; }

        [BsonElement("ok")] // Supported in: 2.6.12, 3.0.14, 3.0.9, 3.2.12, 3.4.2
        public Double Ok { get; set; }


        partial void OnEndInit();
        partial void OnBeginInit();

        void ISupportInitialize.BeginInit()
        {
            OnBeginInit();
        }

        void ISupportInitialize.EndInit()
        {
            OnEndInit();
        }

    }


    // Path: system, Supported in: 2.6.12, 3.0.14, 3.0.9, 3.2.12, 3.4.2
    public partial class System_of_HostInfo : ISupportInitialize
    {

        [BsonElement("currentTime")] // Supported in: 2.6.12, 3.0.14, 3.0.9, 3.2.12, 3.4.2
        public DateTime CurrentTime { get; set; }

        [BsonElement("hostname")] // Supported in: 2.6.12, 3.0.14, 3.0.9, 3.2.12, 3.4.2
        public String Hostname { get; set; }

        [BsonElement("cpuAddrSize")] // Supported in: 2.6.12, 3.0.14, 3.0.9, 3.2.12, 3.4.2
        public Int32 CpuAddrSize { get; set; }

        [BsonElement("memSizeMB")] // Supported in: 2.6.12, 3.0.14, 3.0.9, 3.2.12, 3.4.2
        public Int32 MemSizeMB { get; set; }

        [BsonElement("numCores")] // Supported in: 2.6.12, 3.0.14, 3.0.9, 3.2.12, 3.4.2
        public Int32 NumCores { get; set; }

        [BsonElement("cpuArch")] // Supported in: 2.6.12, 3.0.14, 3.0.9, 3.2.12, 3.4.2
        public String CpuArch { get; set; }

        [BsonElement("numaEnabled")] // Supported in: 2.6.12, 3.0.14, 3.0.9, 3.2.12, 3.4.2
        public Boolean NumaEnabled { get; set; }


        partial void OnEndInit();
        partial void OnBeginInit();

        void ISupportInitialize.BeginInit()
        {
            OnBeginInit();
        }

        void ISupportInitialize.EndInit()
        {
            OnEndInit();
        }

    }


    // Path: os, Supported in: 2.6.12, 3.0.14, 3.0.9, 3.2.12, 3.4.2
    public partial class Os_of_HostInfo : ISupportInitialize
    {

        [BsonElement("type")] // Supported in: 2.6.12, 3.0.14, 3.0.9, 3.2.12, 3.4.2
        public String Type { get; set; }

        [BsonElement("name")] // Supported in: 2.6.12, 3.0.14, 3.0.9, 3.2.12, 3.4.2
        public String Name { get; set; }

        [BsonElement("version")] // Supported in: 2.6.12, 3.0.14, 3.0.9, 3.2.12, 3.4.2
        public String Version { get; set; }


        partial void OnEndInit();
        partial void OnBeginInit();

        void ISupportInitialize.BeginInit()
        {
            OnBeginInit();
        }

        void ISupportInitialize.EndInit()
        {
            OnEndInit();
        }

    }


    // Path: extra, Supported in: 2.6.12, 3.0.14, 3.0.9, 3.2.12, 3.4.2
    public partial class Extra_of_HostInfo : ISupportInitialize
    {

        [BsonElement("pageSize")] // Supported in: 2.6.12, 3.0.14, 3.0.9, 3.2.12, 3.4.2
        public Int64 PageSize { get; set; }

        [BsonElement("versionString")] // Supported in: 3.0.9
        public String VersionString { get; set; }

        [BsonElement("libcVersion")] // Supported in: 3.0.9
        public String LibcVersion { get; set; }

        [BsonElement("kernelVersion")] // Supported in: 3.0.9
        public String KernelVersion { get; set; }

        [BsonElement("cpuFrequencyMHz")] // Supported in: 3.0.9
        public String CpuFrequencyMHz { get; set; }

        [BsonElement("cpuFeatures")] // Supported in: 3.0.9
        public String CpuFeatures { get; set; }

        [BsonElement("numPages")] // Supported in: 3.0.9
        public Int32 NumPages { get; set; }

        [BsonElement("maxOpenFiles")] // Supported in: 3.0.9
        public Int32 MaxOpenFiles { get; set; }


        partial void OnEndInit();
        partial void OnBeginInit();

        void ISupportInitialize.BeginInit()
        {
            OnBeginInit();
        }

        void ISupportInitialize.EndInit()
        {
            OnEndInit();
        }

    }
}