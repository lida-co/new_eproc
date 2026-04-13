using System;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Linq;
using Model.Helper;
using Reston.Pinata.Model.JimbisModel;
using Reston.Pinata.Model.Migrations;

namespace Reston.Pinata.Model.Migrations
{

    public class Configuration : DbMigrationsConfiguration<Reston.Pinata.Model.AppDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;// PengadaanConstants.RunSeeder;//PengadaanConstants.RunSeeder;

           // Seed(new Reston.Pinata.Model.AppDbContext());
        }

        protected override void Seed(Reston.Pinata.Model.AppDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            
            //Seeder.Seed(context);

            //context.ReferenceDatas.AddOrUpdate(
            //    // //jenis asuransi
            //      new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEF_TYPE, Code = "10110", LocalizedName = "Kendaraan Bermotor (KBM)", LocalizedDesc = "Kendaraan Bermotor (KBM)" }
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEF_TYPE, Code = "10120", LocalizedName = "Aset", LocalizedDesc = "Aset" }
            //    //Coverage Asuransi
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEF_COVERAGE, Code = "20110", LocalizedName = "All-Risk / Comprehensive", LocalizedDesc = "All-Risk / Comprehensive", StringAttr1 = "10110" }
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEF_COVERAGE, Code = "20120", LocalizedName = "Total Lost Only / TLO", LocalizedDesc = "Total Lost Only / TLO", StringAttr1 = "10110" }
            //    ////Region Asuransi
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_REGION, Code = "30110", LocalizedName = "Wilayah I", LocalizedDesc = "Wilayah I", StringAttr1 = "10110" }
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_REGION, Code = "30120", LocalizedName = "Wilayah II", LocalizedDesc = "Wilayah II", StringAttr1 = "10110" }
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_REGION, Code = "30130", LocalizedName = "Wilayah III", LocalizedDesc = "Wilayah III", StringAttr1 = "10110" }
            //    //Benefit Asuransi
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEFIT, Code = "40101", LocalizedName = "Kategori 1 - Kendaraan Penumpang Non-Bus, Non-Truk - UP 0 s/d 150jt", LocalizedDesc = "Kategori 1 - Kendaraan Penumpang Non-Bus, Non-Truk - UP 0 s/d 150jt", StringAttr1 = "10110", FlagAttr1 = true }
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEFIT, Code = "40102", LocalizedName = "Kategori 2 - Kendaraan Penumpang Non-Bus, Non-Truk - UP 150jt s/d 200jt", LocalizedDesc = "Kategori 2 - Kendaraan Penumpang Non-Bus, Non-Truk - UP 150jt s/d 200jt", StringAttr1 = "10110", FlagAttr1 = true }
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEFIT, Code = "40103", LocalizedName = "Kategori 3 - Kendaraan Penumpang Non-Bus, Non-Truk - UP 200jt s/d 400jt", LocalizedDesc = "Kategori 3 - Kendaraan Penumpang Non-Bus, Non-Truk - UP 200jt s/d 400jt", StringAttr1 = "10110", FlagAttr1 = true }
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEFIT, Code = "40104", LocalizedName = "Kategori 4 - Kendaraan Penumpang Non-Bus, Non-Truk - UP 400jt s/d 800jt", LocalizedDesc = "Kategori 4 - Kendaraan Penumpang Non-Bus, Non-Truk - UP 400jt s/d 800jt", StringAttr1 = "10110", FlagAttr1 = true }
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEFIT, Code = "40105", LocalizedName = "Kategori 5 - Kendaraan Penumpang Non-Bus, Non-Truk - UP diatas 800jt", LocalizedDesc = "Kategori 5 - Kendaraan Penumpang Non-Bus, Non-Truk - UP diatas 800jt", StringAttr1 = "10110", FlagAttr1 = true }
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEFIT, Code = "40106", LocalizedName = "Kategori 6 - Kendaraan Truk, Pickup - Semua UP", LocalizedDesc = "Kategori 6 - Kendaraan Truk, Pickup - Semua UP", StringAttr1 = "10110", FlagAttr1 = true }
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEFIT, Code = "40107", LocalizedName = "Kategori 7 - Kendaraan Bus - Semua UP", LocalizedDesc = "Kategori 7 - Kendaraan Bus - Semua UP", StringAttr1 = "10110", FlagAttr1 = true }
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEFIT, Code = "40108", LocalizedName = "Kategori 8 - Kendaraan Roda Dua - Semua UP", LocalizedDesc = "Kategori 8 - Kendaraan Roda Dua - Semua UP", StringAttr1 = "10110", FlagAttr1 = true }
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEFIT, Code = "40109", LocalizedName = "Banjir termasuk Angin Topan", LocalizedDesc = "Banjir termasuk Angin Topan", StringAttr1 = "10110", FlagAttr1 = false }
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEFIT, Code = "40110", LocalizedName = "Gempa Bumi, Tsunami", LocalizedDesc = "Gempa Bumi, Tsunami", StringAttr1 = "10110", FlagAttr1 = false }
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEFIT, Code = "40111", LocalizedName = "Huru hara dan Kerusuhan (SRCC)", LocalizedDesc = "Huru hara dan Kerusuhan (SRCC)", StringAttr1 = "10110", FlagAttr1 = false }
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEFIT, Code = "40112", LocalizedName = "Terorisme dan Sabotase", LocalizedDesc = "Terorisme dan Sabotase", StringAttr1 = "10110", FlagAttr1 = false }
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEFIT, Code = "40113", LocalizedName = "Tanggungjawab Hukum Pihak Ketiga (Kendaraan Penumpang dan Sepeda Motor) - UP hingga 25jt", LocalizedDesc = "Tanggungjawab Hukum Pihak Ketiga (Kendaraan Penumpang dan Sepeda Motor) - UP hingga 25jt", StringAttr1 = "10110", FlagAttr1 = false }
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEFIT, Code = "40114", LocalizedName = "Tanggungjawab Hukum Pihak Ketiga (Kendaraan Penumpang dan Sepeda Motor) - UP 25jt s/d 50jt", LocalizedDesc = "Tanggungjawab Hukum Pihak Ketiga (Kendaraan Penumpang dan Sepeda Motor) - UP 25jt s/d 50jt", StringAttr1 = "10110", FlagAttr1 = false }
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEFIT, Code = "40115", LocalizedName = "Tanggungjawab Hukum Pihak Ketiga (Kendaraan Penumpang dan Sepeda Motor) - UP 50jt s/d 100jt", LocalizedDesc = "Tanggungjawab Hukum Pihak Ketiga (Kendaraan Penumpang dan Sepeda Motor) - UP 50jt s/d 100jt", StringAttr1 = "10110", FlagAttr1 = false }
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEFIT, Code = "40116", LocalizedName = "Tanggungjawab Hukum Pihak Ketiga (Kendaraan Penumpang dan Sepeda Motor) - UP diatas 100jt", LocalizedDesc = "Tanggungjawab Hukum Pihak Ketiga (Kendaraan Penumpang dan Sepeda Motor) - UP diatas 100jt", StringAttr1 = "10110", FlagAttr1 = false }
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEFIT, Code = "40117", LocalizedName = "Tanggung Jawab Hukum terhadap Pihak ketiga (Kendaraan Niaga, Truk, dan Bus) - UP hingga 25jt", LocalizedDesc = "Tanggung Jawab Hukum terhadap Pihak ketiga (Kendaraan Niaga, Truk, dan Bus) - UP hingga 25jt", StringAttr1 = "10110", FlagAttr1 = false }
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEFIT, Code = "40118", LocalizedName = "Tanggung Jawab Hukum terhadap Pihak ketiga (Kendaraan Niaga, Truk, dan Bus) - UP 25jt s/d 50jt", LocalizedDesc = "Tanggung Jawab Hukum terhadap Pihak ketiga (Kendaraan Niaga, Truk, dan Bus) - UP 25jt s/d 50jt", StringAttr1 = "10110", FlagAttr1 = false }
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEFIT, Code = "40119", LocalizedName = "Tanggung Jawab Hukum terhadap Pihak ketiga (Kendaraan Niaga, Truk, dan Bus) - UP 50jt s/d 100jt", LocalizedDesc = "Tanggung Jawab Hukum terhadap Pihak ketiga (Kendaraan Niaga, Truk, dan Bus) - UP 50jt s/d 100jt", StringAttr1 = "10110", FlagAttr1 = false }
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEFIT, Code = "40120", LocalizedName = "Tanggung Jawab Hukum terhadap Pihak ketiga (Kendaraan Niaga, Truk, dan Bus) - UP diatas 100jt", LocalizedDesc = "Tanggung Jawab Hukum terhadap Pihak ketiga (Kendaraan Niaga, Truk, dan Bus) - UP diatas 100jt", StringAttr1 = "10110", FlagAttr1 = false }
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEFIT, Code = "40121", LocalizedName = "Kecelakaan Diri untuk Penumpang - Untuk Pengemudi - (dari uang pertanggungan kecelakaan diri)", LocalizedDesc = "Kecelakaan Diri untuk Penumpang - Untuk Pengemudi - (dari uang pertanggungan kecelakaan diri)", StringAttr1 = "10110", FlagAttr1 = false }
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEFIT, Code = "40122", LocalizedName = "Kecelakaan Diri untuk Penumpang - Untuk Penumpang - (dari uang pertanggungan kecelakaan diri untuk setiap tempat duduk penumpang)", LocalizedDesc = "Kecelakaan Diri untuk Penumpang - Untuk Penumpang - (dari uang pertanggungan kecelakaan diri untuk setiap tempat duduk penumpang)", StringAttr1 = "10110", FlagAttr1 = false }
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEFIT, Code = "40123", LocalizedName = "Tanggung Jawab Hukum terhadap Penumpang - UP hingga 25jt", LocalizedDesc = "Tanggung Jawab Hukum terhadap Penumpang - UP hingga 25jt", StringAttr1 = "10110", FlagAttr1 = false }
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEFIT, Code = "40124", LocalizedName = "Tanggung Jawab Hukum terhadap Penumpang - UP 25jt s/d 50jt", LocalizedDesc = "Tanggung Jawab Hukum terhadap Penumpang - UP 25jt s/d 50jt", StringAttr1 = "10110", FlagAttr1 = false }
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEFIT, Code = "40125", LocalizedName = "Tanggung Jawab Hukum terhadap Penumpang - UP 50jt s/d 100jt", LocalizedDesc = "Tanggung Jawab Hukum terhadap Penumpang - UP 50jt s/d 100jt", StringAttr1 = "10110", FlagAttr1 = false }
            //    , new ReferenceData { Qualifier = RefDataQualifier.QUALIFIER_INS_BENEFIT, Code = "40126", LocalizedName = "Tanggung Jawab Hukum terhadap Penumpang - UP diatas 100jt", LocalizedDesc = "Tanggung Jawab Hukum terhadap Penumpang - UP diatas 100jt", StringAttr1 = "10110", FlagAttr1 = false }
            //    );

            //Seeder.Seed(context);
        }
    }
}
