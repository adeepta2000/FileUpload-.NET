using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Fileupload.Models
{
    public class DocumentDTO
    {
        public int DocumentId { get; set; }
    
        public string FileName { get; set; }
    
        public string FilePath { get; set; }

        public System.DateTime UploadDate { get; set; }
 
        public string StudentId { get; set; }
    }
}