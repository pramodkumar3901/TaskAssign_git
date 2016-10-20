namespace TaskAssign.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update3 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Member", "test");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Member", "test", c => c.String());
        }
    }
}
