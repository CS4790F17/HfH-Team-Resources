using HabitatForHumanity.Models;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HabitatForHumanity.ViewModels
{
    public class CategorySearchModel
    {
        public int? Page { get; set; }
        public IPagedList<ProjectCategory> SearchResults { get; set; }

    }
}