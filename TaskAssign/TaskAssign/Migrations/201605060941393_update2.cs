namespace TaskAssign.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Member", "test", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Member", "test");
        }
    }
}
