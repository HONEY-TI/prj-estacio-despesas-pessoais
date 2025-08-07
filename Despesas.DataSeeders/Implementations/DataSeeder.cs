using EasyCryptoSalt;
using Repository;

namespace DataSeeders.Implementations;
public class DataSeeder : IDataSeeder
{
    private readonly ICrypto _crypto;

    private readonly RegisterContext _context;
    public DataSeeder(RegisterContext context, ICrypto crypto)
    {
        _context = context;
        _crypto = crypto;
    }
    public void SeedData()
    {
        try
        {

            new DataSeederControleAcesso(_context, _crypto).SeedData();
            new DataSeederDespesa(_context).SeedData();
            new DataSeederReceita(_context).SeedData();
        }
        catch { throw; }
    }
}
