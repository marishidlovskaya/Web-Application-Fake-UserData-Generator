using Microsoft.EntityFrameworkCore;
using System;

namespace FakeUserDataGenerator.Data
{
    public class PopulateDatabase
    {

        private readonly UserContext _context;

        public PopulateDatabase(UserContext context)
        {
            _context = context;
        }
        public UserContext Get_context()
        {
            return _context;
        }

        public void PopulateTable<T>(List<T> listOfData)
        {
            foreach (var item in listOfData)
            {
                if (item == null)
                {
                    continue;
                }
                _context.Add(item);             
            }
            _context.SaveChanges();
        }
    }
}
