using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFCore.SaveChangesInterceptorTwoFases;

public class Blog
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public ICollection<Post> Posts { get; set; }
}

public class Post
{
    public int Id { get; set; }

    public string Title { get; set; }

    public string Body { get; set; }

    public DateTime DateTime { get; set; }

    public Blog Blog { get;}
}
