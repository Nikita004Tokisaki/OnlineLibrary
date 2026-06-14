using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OnlineLibrary;

namespace OnlineLibrary.Helpers
{
    public static class SessionHelper
    {
        public static Users CurrentUser { get; set; }
        public static bool IsAdmin => CurrentUser?.Role == "admin";
        public static bool IsAuthor => CurrentUser?.Role == "author";
        public static bool IsUser => CurrentUser?.Role == "user";
        public static bool IsFrozen => CurrentUser?.Status == "frozen";
        public static void Clear() => CurrentUser = null;
    }
}