using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace http_pratice.Modles
{
    internal class Routes
    {
        public const string DeleteStudent = "delStudentInfo/{id}";
        public const string UpdateStudent = "updateStudentInfo/{id}";
        public const string PostStudent = "PostStudentInfo";
        public const string GetStudentId = "GetStudentInfoById/{id}";
        public const string GetStudent = "getallstudentinfo";
    }
}
