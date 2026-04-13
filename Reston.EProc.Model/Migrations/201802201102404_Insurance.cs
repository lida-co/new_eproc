namespace Reston.Pinata.Model.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Insurance : DbMigration
    {
        public override void Up()
        {
            DropIndex("catalog.Benefit", new[] { "Nama" });
            CreateTable(
                "asuransi.BenefitRate",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        BenefitCode = c.String(maxLength: 30),
                        RegionCode = c.String(maxLength: 30),
                        IsOpen = c.Boolean(),
                        RateUpperLimit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        RateLowerLimit = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Rate = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsRange = c.Boolean(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "asuransi.InsuranceTarifBenefit",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Guid(nullable: false),
                        BenefitRateId_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("asuransi.BenefitRate", t => t.BenefitRateId_Id)
                .Index(t => t.BenefitRateId_Id);
            
            CreateTable(
                "asuransi.InsuranceTarifBenefitTemplate",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Guid(nullable: false),
                        BenefitRateId_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("asuransi.BenefitRate", t => t.BenefitRateId_Id)
                .Index(t => t.BenefitRateId_Id);
            
            CreateTable(
                "asuransi.InsuranceTarif",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Guid(nullable: false),
                        DocumentTitle = c.String(),
                        CreatedBy = c.String(),
                        CreatedTS = c.DateTime(nullable: false),
                        DeletedBy = c.String(),
                        DeletedTS = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(nullable: false),
                        Owner = c.String(),
                        BenefitType = c.String(),
                        InsuranceTarifTemplateDocumentId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "asuransi.InsuranceTarifTemplate",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DocumentId = c.Guid(nullable: false),
                        DocumentTitle = c.String(),
                        CreatedBy = c.String(),
                        CreatedTS = c.DateTime(nullable: false),
                        DeletedBy = c.String(),
                        DeletedTS = c.DateTime(nullable: false),
                        IsDeleted = c.Boolean(),
                        Owner = c.String(),
                        BenefitType = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            DropTable("catalog.Benefit");
        }
        
        public override void Down()
        {
            CreateTable(
                "catalog.Benefit",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nama = c.String(maxLength: 150),
                        Deskripsi = c.String(maxLength: 255),
                    })
                .PrimaryKey(t => t.Id);
            
            DropForeignKey("asuransi.InsuranceTarifBenefitTemplate", "BenefitRateId_Id", "asuransi.BenefitRate");
            DropForeignKey("asuransi.InsuranceTarifBenefit", "BenefitRateId_Id", "asuransi.BenefitRate");
            DropIndex("asuransi.InsuranceTarifBenefitTemplate", new[] { "BenefitRateId_Id" });
            DropIndex("asuransi.InsuranceTarifBenefit", new[] { "BenefitRateId_Id" });
            DropTable("asuransi.InsuranceTarifTemplate");
            DropTable("asuransi.InsuranceTarif");
            DropTable("asuransi.InsuranceTarifBenefitTemplate");
            DropTable("asuransi.InsuranceTarifBenefit");
            DropTable("asuransi.BenefitRate");
            CreateIndex("catalog.Benefit", "Nama");
        }
    }
}
