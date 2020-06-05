using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BinListWrapperApi.Models
{
    public class ApiOkResponse : ApiResponse
    {
        public object Result { get; }
       
        public ApiOkResponse(object result,String Message)
            : base(200,Message)
        {
            Result = result;
            
        }
    }
}
