using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Shared.Responses.ObjectStorage
{
    public class ChunkUploadResponse
    {
        public int TotalUploadedChunks { get; set; }
        public double TotalUploadedSizeMB { get; set; }
    }
}
