using PIM_VIII.API.Entities;

namespace PIM_VIII.API.Persistence
{
    public class CriadorDbContext
    {
        public List<Criador> Criadores { get; set; }
        public CriadorDbContext()
        {
            Criadores = new List<Criador>();
        }
    }
}
