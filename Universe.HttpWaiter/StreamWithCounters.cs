using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace Universe.HttpWaiter
{
    public class StreamWithCounters : Stream
    {
        public readonly Stream BaseStream;
        public long TotalReadBytes { get; private set; } = 0;
        public long TotalWrittenBytes { get; private set; } = 0;
        internal Action StreamClosed;

        public StreamWithCounters(Stream baseStream)
        {
            if (baseStream == null)
                throw new ArgumentNullException("baseStream");

            BaseStream = baseStream;
        }


        public override Task CopyToAsync(Stream destination, int bufferSize, CancellationToken cancellationToken)
        {
            // WAT?
            return BaseStream.CopyToAsync(destination, bufferSize, cancellationToken);
        }

        public override void Flush()
        {
            BaseStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return BaseStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            BaseStream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            var ret = BaseStream.Read(buffer, offset, count);
            TotalReadBytes += ret;
            return ret;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            BaseStream.Write(buffer, offset, count);
            TotalWrittenBytes += count;
        }
        
        public override bool CanRead { get { return BaseStream.CanRead; } }
        public override bool CanSeek { get { return BaseStream.CanSeek; } }
        public override bool CanWrite { get { return BaseStream.CanWrite; } }
        public override bool CanTimeout { get { return BaseStream.CanTimeout; } }
        public override long Length { get { return BaseStream.Length; } }

        public override long Position
        {
            get { return BaseStream.Position; } 
            set { BaseStream.Position = value; }
        }

        private bool disposed = false;
        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                disposed = true;
                var copy = StreamClosed;
                if (copy != null) copy();
                
                BaseStream.Dispose();
            }
            
        }
    }
}