using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Dev.WooAI.EntityFreworkCore;

public class WooAiDbContext(DbContextOptions<WooAiDbContext> dbContextOptions):IdentityDbContext(dbContextOptions);



