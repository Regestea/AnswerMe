using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Models.Shared.Attribute;

namespace Models.Shared.Requests.ObjectStorage
{
    public class FileChunkRequest
    {
        public Guid UploadToken { get; set; }
        [FileSizeMegabyte(0.01,4)]
        public required byte[] Data { get; set; }
    }
}
