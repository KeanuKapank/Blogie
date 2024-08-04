using Blogie.Blogger.Data.Models;
using Blogie.Blogger.Data.Repository;

namespace Blogie.UI.Models.ViewModel
{
    public class PostCategory
    {
        public Post Post { get; set; }
        public IEnumerable<Category> Categories { get; set; }
    }
}
