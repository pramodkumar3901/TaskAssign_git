namespace TaskAssign.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Member",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        UserName = c.String(),
                        Password = c.String(),
                        RoleId = c.Int(nullable: false),
                        CreatedOn = c.DateTime(nullable: false),
                        ModifiedOn = c.DateTime(nullable: false),
                        Active = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.UserId);
            
            CreateTable(
                "dbo.Lookup",
                c => new
                    {
                        LookupId = c.Int(nullable: false, identity: true),
                        LookupName = c.String(),
                        LookupType = c.String(),
                        LookupOrder = c.Int(nullable: false),
                        ParentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.LookupId);
            
            CreateTable(
                "dbo.Task",
                c => new
                    {
                        TaskId = c.Int(nullable: false, identity: true),
                        TaskType = c.Int(nullable: false),
                        Title = c.String(),
                        Description = c.String(),
                        PostedDate = c.DateTime(nullable: false, storeType: "datetime2"),
                        PostedTime = c.Time(nullable: false),
                        PostedBy = c.Int(nullable: false),
                        AssignedBy = c.Int(nullable: false),
                        ModuleName = c.String(),
                        Priority = c.Int(nullable: false),
                        AssignedTo = c.Int(nullable: false),
                        AssignedOn = c.DateTime(nullable: false, storeType: "datetime2"),
                        AllottedHrs = c.Int(nullable: false),
                        DueDate = c.DateTime(nullable: false, storeType: "datetime2"),
                        StartedDate = c.DateTime(nullable: false, storeType: "datetime2"),
                        ClosedDate = c.DateTime(nullable: false, storeType: "datetime2"),
                        DevStatus = c.Int(nullable: false),
                        InitStatus = c.Int(nullable: false),
                        TesterStatus = c.Int(nullable: false),
                        RejectReason = c.String(),
                        Comments = c.String(),
                        RequestFlag = c.Boolean(nullable: false),
                        RequestHours = c.Int(nullable: false),
                        RequestReason = c.String(),
                    })
                .PrimaryKey(t => t.TaskId);
            
            CreateTable(
                "dbo.History",
                c => new
                    {
                        HistoryId = c.Int(nullable: false, identity: true),
                        TaskId = c.Int(nullable: false),
                        Messages = c.String(),
                    })
                .PrimaryKey(t => t.HistoryId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.History");
            DropTable("dbo.Task");
            DropTable("dbo.Lookup");
            DropTable("dbo.Member");
        }
    }
}
