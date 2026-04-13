using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Reston.Pinata.WebService.Helper;

namespace Reston.EProc.Web.Base.ViewModels
{
    public class BaseRequest
    {
        public BaseHeader Header { get; set; }
    }

    public class BaseResponse
    {
        public BaseHeader Header { get; set; }
    }

    public class BaseHeader : ICloneable
    {
        public string UserId { get; set; }

        public StatusCode StatusCode { get; set; }

        public string StatusMessage { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public class BaseFilteredRequest : BaseRequest
    {
        public virtual BaseFilter Filter { get; set; }
    }

    public class BaseFilteredResponse : BaseResponse
    {
        public virtual BaseFilter Filter { get; set; }
    }


    public class BaseFilter
    {
        public BaseFilter()
        {
            SortColumns = new List<int>();
            SortOrder = new List<int>();
        }

        public int? TotalRecordsCount { get; set; }

        public int? FilteredRecordsCount { get; set; }

        public int? RecordsCountPerPage { get; set; }

        public int? Offset { get; set; }

        public int? PageLength { get; set; }

        public ICollection<int> SortColumns { get; set; }

        public ICollection<int> SortOrder { get; set; }

        public int? DrawCount { get; set; }

    }


    public enum SortOrder 
    { 
        ASCENDING = 4,
        DESCENDING = 9
    }

    public enum StatusCode 
    { 
        // 200s
        SUCCESS,
        // 400s
        BAD_REQUEST,
        NOT_FOUND,
        // 500s
        SERVICE_ERROR,
    }

    


}
