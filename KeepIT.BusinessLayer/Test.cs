using KeepIT.DataAccessLayer.EntityFramework;
using KeepIT.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeepIT.BusinessLayer
{
    public class Test
    {
        private Repository<KeepITUser> repo_user = new Repository<KeepITUser>();
        private Repository<Category> repo_category = new Repository<Category>();
        private Repository<Comment> repo_comment = new Repository<Comment>();
        private Repository<Note> repo_note = new Repository<Note>();

        public Test()
        {
            //DataAccessLayer.DatabaseContext db = new DataAccessLayer.DatabaseContext();
            //db.Categories.ToList();


            List<Category> cats = repo_category.List();
            //List<Category> cats_filtered = repo_category.List(x => x.Id > 5);

        }

        public void InsertTest()
        {
            int result = repo_user.Insert(new KeepITUser()
            {
                Name = "test_Ali",
                Surname = "test_Doğan",
                Email = "test_alidogan915@hotmail.com",
                ActivateGuid = Guid.NewGuid(),
                IsActive = true,
                IsAdmin = true,
                Username = "test_alidogan",
                Password = "2018",
                //ProfileImageFilename = "user_boy.png",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now.AddMinutes(65),
                ModifiedUsername = "test_alidogan"
            });
        }

        public void UpdateTest()
        {
            KeepITUser user = repo_user.Find(x => x.Username == "test_alidogan");

            if (user != null)
            {
                user.Username = "test_updated_alidogan";
                int result = repo_user.Update(user);
            }
        }

        public void DeleteTest()
        {
            KeepITUser user = repo_user.Find(x => x.Username == "test_updated_alidogan");

            if (user != null)
            {
                int result = repo_user.Delete(user);
            }
        }

        public void CommentTest()
        {
            KeepITUser user = repo_user.Find(x => x.Id == 1);
            Note note = repo_note.Find(x => x.Id == 3);

            Comment comment = new Comment()
            {
                Text = "Bu bir testtir.",
                CreatedOn = DateTime.Now,
                ModifiedOn = DateTime.Now,
                ModifiedUsername = "alidogan",
                Note = note,
                Owner = user
            };

            repo_comment.Insert(comment);
        }
    }
}
