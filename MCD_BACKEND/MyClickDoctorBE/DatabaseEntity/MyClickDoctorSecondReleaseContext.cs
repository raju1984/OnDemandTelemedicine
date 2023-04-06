//using System;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Metadata;

//namespace MyClickDoctorBE.DatabaseEntity
//{
//    public partial class MyClickDoctorSecondReleaseContext : DbContext
//    {
//        public MyClickDoctorSecondReleaseContext()
//        {
//        }

//        public MyClickDoctorSecondReleaseContext(DbContextOptions<MyClickDoctorSecondReleaseContext> options)
//            : base(options)
//        {
//        }

//        public virtual DbSet<ActivityList> ActivityList { get; set; }
//        public virtual DbSet<Administrator> Administrator { get; set; }
//        public virtual DbSet<AmamnesticDataPreAppointment> AmamnesticDataPreAppointment { get; set; }
//        public virtual DbSet<Appoinments> Appoinments { get; set; }
//        public virtual DbSet<AppointmentDocuments> AppointmentDocuments { get; set; }
//        public virtual DbSet<Chat> Chat { get; set; }
//        public virtual DbSet<ChatLog> ChatLog { get; set; }
//        public virtual DbSet<ClinicDocMap> ClinicDocMap { get; set; }
//        public virtual DbSet<Currency> Currency { get; set; }
//        public virtual DbSet<DocAward> DocAward { get; set; }
//        public virtual DbSet<DoctorAvailibiltyforAppointment> DoctorAvailibiltyforAppointment { get; set; }
//        public virtual DbSet<DoctorCategoryType> DoctorCategoryType { get; set; }
//        public virtual DbSet<DoctorClinicMap> DoctorClinicMap { get; set; }
//        public virtual DbSet<DoctorNurseAvailability> DoctorNurseAvailability { get; set; }
//        public virtual DbSet<DoctorPhramaRepMap> DoctorPhramaRepMap { get; set; }
//        public virtual DbSet<DoctorReview> DoctorReview { get; set; }
//        public virtual DbSet<Doctors> Doctors { get; set; }
//        public virtual DbSet<Doctorspecilization> Doctorspecilization { get; set; }
//        public virtual DbSet<Documents> Documents { get; set; }
//        public virtual DbSet<Faqs> Faqs { get; set; }
//        public virtual DbSet<Gdprsettings> Gdprsettings { get; set; }
//        public virtual DbSet<InvitationMaster> InvitationMaster { get; set; }
//        public virtual DbSet<InvitationUserMap> InvitationUserMap { get; set; }
//        public virtual DbSet<IssueDescription> IssueDescription { get; set; }
//        public virtual DbSet<Morbidity> Morbidity { get; set; }
//        public virtual DbSet<NewDoctorInvitationMaster> NewDoctorInvitationMaster { get; set; }
//        public virtual DbSet<NotificationBox> NotificationBox { get; set; }
//        public virtual DbSet<NotificationToken> NotificationToken { get; set; }
//        public virtual DbSet<NurseAvailibiltyforAppointment> NurseAvailibiltyforAppointment { get; set; }
//        public virtual DbSet<NurseConsultRequest> NurseConsultRequest { get; set; }
//        public virtual DbSet<Nurses> Nurses { get; set; }
//        public virtual DbSet<Patient> Patient { get; set; }
//        public virtual DbSet<PatientMedicalHistory> PatientMedicalHistory { get; set; }
//        public virtual DbSet<PatientMediHistoryDocuments> PatientMediHistoryDocuments { get; set; }
//        public virtual DbSet<PaymentMethod> PaymentMethod { get; set; }
//        public virtual DbSet<Payments> Payments { get; set; }
//        public virtual DbSet<PaymentsLog> PaymentsLog { get; set; }
//        public virtual DbSet<PharmaceuticalsCompany> PharmaceuticalsCompany { get; set; }
//        public virtual DbSet<PharmaCompanyRepMap> PharmaCompanyRepMap { get; set; }
//        public virtual DbSet<PharmaRepDocumentMap> PharmaRepDocumentMap { get; set; }
//        public virtual DbSet<PharmaRepresentative> PharmaRepresentative { get; set; }
//        public virtual DbSet<PriceListMaster> PriceListMaster { get; set; }
//        public virtual DbSet<SpecializationDetailsAndDiploma> SpecializationDetailsAndDiploma { get; set; }
//        public virtual DbSet<SpecilizationCategory> SpecilizationCategory { get; set; }
//        public virtual DbSet<TempDoctors> TempDoctors { get; set; }
//        public virtual DbSet<TempNurses> TempNurses { get; set; }
//        public virtual DbSet<TransferAppoinmentDetails> TransferAppoinmentDetails { get; set; }
//        public virtual DbSet<Transfers> Transfers { get; set; }
//        public virtual DbSet<UserGroup> UserGroup { get; set; }
//        public virtual DbSet<Users> Users { get; set; }
//        public virtual DbSet<Workplace> Workplace { get; set; }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            //if (!optionsBuilder.IsConfigured)
//            //{
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
//               
//           // }
//        }

//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            modelBuilder.Entity<ActivityList>(entity =>
//            {
//                entity.HasOne(d => d.Appointment)
//                    .WithMany(p => p.ActivityList)
//                    .HasForeignKey(d => d.AppointmentId)
//                    .OnDelete(DeleteBehavior.ClientSetNull)
//                    .HasConstraintName("FK_ActivityList_Appoinments");
//            });

//            modelBuilder.Entity<Administrator>(entity =>
//            {
//                entity.Property(e => e.ContractEndDate).HasColumnType("datetime");

//                entity.Property(e => e.ContractStartDate).HasColumnType("datetime");

//                entity.Property(e => e.Mshcid).HasColumnName("MSHCID");
//            });

//            modelBuilder.Entity<Appoinments>(entity =>
//            {
//                entity.Property(e => e.Appoinmentcode)
//                    .HasMaxLength(50)
//                    .IsUnicode(false);

//                entity.Property(e => e.AppointmentTitle).HasMaxLength(50);

//                entity.Property(e => e.BookingDate).HasColumnType("datetime");

//                entity.Property(e => e.DocId).HasColumnName("docId");

//                entity.Property(e => e.EndDate).HasColumnType("datetime");

//                entity.Property(e => e.LastVisitedDate).HasColumnType("datetime");

//                entity.Property(e => e.NextConsultationDate).HasColumnType("datetime");

//                entity.Property(e => e.NurseStartDate).HasColumnType("datetime");

//                entity.Property(e => e.StartDate).HasColumnType("datetime");

//                entity.Property(e => e.TimeZone).HasMaxLength(10);

//                entity.HasOne(d => d.DocSlot)
//                    .WithMany(p => p.Appoinments)
//                    .HasForeignKey(d => d.DocSlotId)
//                    .HasConstraintName("FK_Appoinments_DoctorAvailibiltyforAppointment");

//                entity.HasOne(d => d.Doctor)
//                    .WithMany(p => p.Appoinments)
//                    .HasForeignKey(d => d.DoctorId)
//                    .HasConstraintName("FK_Appoinments_Doctors");

//                entity.HasOne(d => d.Nurse)
//                    .WithMany(p => p.Appoinments)
//                    .HasForeignKey(d => d.NurseId)
//                    .HasConstraintName("FK_Appoinments_Nurses");

//                entity.HasOne(d => d.Patient)
//                    .WithMany(p => p.Appoinments)
//                    .HasForeignKey(d => d.PatientId)
//                    .HasConstraintName("FK_Appoinments_Patient");

//                entity.HasOne(d => d.PharmaRep)
//                    .WithMany(p => p.Appoinments)
//                    .HasForeignKey(d => d.PharmaRepId)
//                    .HasConstraintName("FK_Appoinments_PharmaRepresentative");
//            });

//            modelBuilder.Entity<AppointmentDocuments>(entity =>
//            {
//                entity.HasOne(d => d.Appointmet)
//                    .WithMany(p => p.AppointmentDocuments)
//                    .HasForeignKey(d => d.AppointmetId)
//                    .HasConstraintName("FK_AppointmentDocuments_Appoinments");

//                entity.HasOne(d => d.Document)
//                    .WithMany(p => p.AppointmentDocuments)
//                    .HasForeignKey(d => d.DocumentId)
//                    .HasConstraintName("FK_AppointmentDocuments_Documents");
//            });

//            modelBuilder.Entity<Chat>(entity =>
//            {
//                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

//                entity.Property(e => e.Msmanagement).HasColumnName("MSManagement");

//                entity.Property(e => e.Msoperator).HasColumnName("MSOperator");
//            });

//            modelBuilder.Entity<ChatLog>(entity =>
//            {
//                entity.Property(e => e.ChatTime).HasColumnType("datetime");

//                entity.Property(e => e.Mesaage).IsRequired();
//            });

//            modelBuilder.Entity<ClinicDocMap>(entity =>
//            {
//                entity.Property(e => e.Image)
//                    .HasMaxLength(150)
//                    .IsUnicode(false);
//            });

//            modelBuilder.Entity<Currency>(entity =>
//            {
//                entity.Property(e => e.Id).ValueGeneratedNever();

//                entity.Property(e => e.CurrencyCode).HasMaxLength(10);

//                entity.Property(e => e.CurrencyName).HasMaxLength(50);
//            });

//            modelBuilder.Entity<DocAward>(entity =>
//            {
//                entity.Property(e => e.AwardTitle).HasMaxLength(250);
//            });

//            modelBuilder.Entity<DoctorAvailibiltyforAppointment>(entity =>
//            {
//                entity.Property(e => e.FromTime).HasColumnType("datetime");

//                entity.Property(e => e.ToTime).HasColumnType("datetime");
//            });

//            modelBuilder.Entity<DoctorCategoryType>(entity =>
//            {
//                entity.Property(e => e.HunName).HasMaxLength(150);

//                entity.Property(e => e.Name).HasMaxLength(150);
//            });

//            modelBuilder.Entity<DoctorNurseAvailability>(entity =>
//            {
//                entity.Property(e => e.DayofWeek)
//                    .HasMaxLength(20)
//                    .IsUnicode(false);
//            });

//            modelBuilder.Entity<DoctorPhramaRepMap>(entity =>
//            {
//                entity.Property(e => e.Created).HasColumnType("datetime");

//                entity.HasOne(d => d.Cat)
//                    .WithMany(p => p.DoctorPhramaRepMap)
//                    .HasForeignKey(d => d.CatId)
//                    .OnDelete(DeleteBehavior.ClientSetNull)
//                    .HasConstraintName("FK_DoctorPhramaRepMap_SpecilizationCategory");
//            });

//            modelBuilder.Entity<DoctorReview>(entity =>
//            {
//                entity.Property(e => e.Id).ValueGeneratedNever();

//                entity.Property(e => e.Created).HasColumnType("datetime");

//                entity.Property(e => e.DocId).ValueGeneratedOnAdd();

//                entity.Property(e => e.Review).IsRequired();

//                entity.HasOne(d => d.Doc)
//                    .WithMany(p => p.DoctorReview)
//                    .HasForeignKey(d => d.DocId)
//                    .OnDelete(DeleteBehavior.ClientSetNull)
//                    .HasConstraintName("FK_DoctorReview_Doctors");

//                entity.HasOne(d => d.IdNavigation)
//                    .WithOne(p => p.InverseIdNavigation)
//                    .HasForeignKey<DoctorReview>(d => d.Id)
//                    .OnDelete(DeleteBehavior.ClientSetNull)
//                    .HasConstraintName("FK_DoctorReview_DoctorReview");
//            });

//            modelBuilder.Entity<Doctors>(entity =>
//            {
//                entity.Property(e => e.ContarctEndDate).HasColumnType("datetime");

//                entity.Property(e => e.ContractStartDate).HasColumnType("datetime");

//                entity.Property(e => e.Country).HasMaxLength(50);

//                entity.Property(e => e.Dob)
//                    .HasColumnName("DOB")
//                    .HasColumnType("datetime");

//                entity.Property(e => e.FirstName).IsRequired();

//                entity.Property(e => e.Gander).HasMaxLength(50);

//                entity.Property(e => e.Latitude).HasMaxLength(100);

//                entity.Property(e => e.Longitude).HasMaxLength(100);

//                entity.Property(e => e.MedicalRegistrationNo)
//                    .IsRequired()
//                    .HasMaxLength(100);

//                entity.Property(e => e.PhoneNo).HasMaxLength(50);

//                entity.Property(e => e.PhotoUrl).HasColumnName("PhotoURL");

//                entity.Property(e => e.RegistrationNo).HasMaxLength(550);

//                entity.Property(e => e.State).HasMaxLength(50);

//                entity.Property(e => e.StreetNumber).HasMaxLength(50);

//                entity.Property(e => e.Zipcode)
//                    .HasMaxLength(20)
//                    .IsUnicode(false);

//                entity.HasOne(d => d.User)
//                    .WithMany(p => p.Doctors)
//                    .HasForeignKey(d => d.UserId)
//                    .OnDelete(DeleteBehavior.ClientSetNull)
//                    .HasConstraintName("FK_Doctors_Users");
//            });

//            modelBuilder.Entity<Doctorspecilization>(entity =>
//            {
//                entity.Property(e => e.Name).HasMaxLength(250);

//                entity.HasOne(d => d.Doc)
//                    .WithMany(p => p.Doctorspecilization)
//                    .HasForeignKey(d => d.DocId)
//                    .OnDelete(DeleteBehavior.ClientSetNull)
//                    .HasConstraintName("FK_Doctorspecilization_Doctors1");
//            });

//            modelBuilder.Entity<Documents>(entity =>
//            {
//                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

//                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

//                entity.Property(e => e.Title).HasMaxLength(50);

//                entity.Property(e => e.UpdateAt).HasColumnType("datetime");

//                entity.Property(e => e.Url).HasColumnName("URL");
//            });

//            modelBuilder.Entity<Faqs>(entity =>
//            {
//                entity.ToTable("FAQs");
//            });

//            modelBuilder.Entity<Gdprsettings>(entity =>
//            {
//                entity.ToTable("GDPRSettings");

//                entity.Property(e => e.Ipaddress).HasColumnName("IPAddress");
//            });

//            modelBuilder.Entity<InvitationMaster>(entity =>
//            {
//                entity.Property(e => e.Id).ValueGeneratedNever();

//                entity.Property(e => e.InvitationCode).HasMaxLength(10);

//                entity.Property(e => e.Title).HasMaxLength(50);
//            });

//            modelBuilder.Entity<InvitationUserMap>(entity =>
//            {
//                entity.Property(e => e.Email).HasMaxLength(256);
//            });

//            modelBuilder.Entity<IssueDescription>(entity =>
//            {
//                entity.Property(e => e.Created).HasColumnType("datetime");
//            });

//            modelBuilder.Entity<Morbidity>(entity =>
//            {
//                entity.Property(e => e.Anamnesis).HasColumnName("anamnesis");

//                entity.Property(e => e.Created).HasColumnType("datetime");

//                entity.Property(e => e.Diagnosis).HasColumnName("diagnosis");

//                entity.Property(e => e.Therapy).HasColumnName("therapy");
//            });

//            modelBuilder.Entity<NewDoctorInvitationMaster>(entity =>
//            {
//                entity.Property(e => e.Email).HasMaxLength(50);

//                entity.Property(e => e.FirstName).HasMaxLength(256);

//                entity.Property(e => e.LastName).HasMaxLength(256);

//                entity.Property(e => e.Specility).HasMaxLength(256);
//            });

//            modelBuilder.Entity<NotificationBox>(entity =>
//            {
//                entity.Property(e => e.MeetingId)
//                    .IsRequired()
//                    .HasMaxLength(50);

//                entity.Property(e => e.ReqId).HasMaxLength(150);

//                entity.HasOne(d => d.Admin)
//                    .WithMany(p => p.NotificationBox)
//                    .HasForeignKey(d => d.AdminId)
//                    .HasConstraintName("FK_NotificationBox_Administrator");

//                entity.HasOne(d => d.User)
//                    .WithMany(p => p.NotificationBox)
//                    .HasForeignKey(d => d.UserId)
//                    .OnDelete(DeleteBehavior.ClientSetNull)
//                    .HasConstraintName("FK_NotificationBox_Users");
//            });

//            modelBuilder.Entity<NotificationToken>(entity =>
//            {
//                entity.HasOne(d => d.User)
//                    .WithMany(p => p.NotificationToken)
//                    .HasForeignKey(d => d.UserId)
//                    .OnDelete(DeleteBehavior.ClientSetNull)
//                    .HasConstraintName("FK_NotificationToken_Users");
//            });

//            modelBuilder.Entity<NurseAvailibiltyforAppointment>(entity =>
//            {
//                entity.Property(e => e.EndTime).HasColumnType("datetime");

//                entity.Property(e => e.StartTime).HasColumnType("datetime");
//            });

//            modelBuilder.Entity<NurseConsultRequest>(entity =>
//            {
//                entity.Property(e => e.DaysofWeek).HasMaxLength(50);
//            });

//            modelBuilder.Entity<Nurses>(entity =>
//            {
//                entity.Property(e => e.City).HasMaxLength(150);

//                entity.Property(e => e.ContractEndDate).HasColumnType("datetime");

//                entity.Property(e => e.ContractStartDate).HasColumnType("datetime");

//                entity.Property(e => e.Country).HasMaxLength(150);

//                entity.Property(e => e.FirstName).IsRequired();

//                entity.Property(e => e.Gender).HasMaxLength(50);

//                entity.Property(e => e.MobileNo).HasMaxLength(50);

//                entity.Property(e => e.ProfileUrl).HasMaxLength(150);

//                entity.Property(e => e.ShortDescription).IsRequired();

//                entity.Property(e => e.ZipCode)
//                    .HasMaxLength(50)
//                    .IsUnicode(false);
//            });

//            modelBuilder.Entity<Patient>(entity =>
//            {
//                entity.Property(e => e.City)
//                    .IsRequired()
//                    .HasMaxLength(256);

//                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

//                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

//                entity.Property(e => e.Dob)
//                    .HasColumnName("DOB")
//                    .HasColumnType("date");

//                entity.Property(e => e.FirstName).IsRequired();

//                entity.Property(e => e.Gender)
//                    .IsRequired()
//                    .HasMaxLength(50)
//                    .IsUnicode(false);

//                entity.Property(e => e.ShortIntroduction).IsRequired();

//                entity.Property(e => e.SocialSecurityNumber)
//                    .IsRequired()
//                    .HasMaxLength(50);

//                entity.Property(e => e.StreetNumber).HasMaxLength(256);

//                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");

//                entity.Property(e => e.Zipcode)
//                    .IsRequired()
//                    .HasMaxLength(50)
//                    .IsUnicode(false);
//            });

//            modelBuilder.Entity<PatientMedicalHistory>(entity =>
//            {
//                entity.Property(e => e.Date).HasColumnType("datetime");

//                entity.Property(e => e.Description).IsRequired();
//            });

//            modelBuilder.Entity<PaymentMethod>(entity =>
//            {
//                entity.Property(e => e.Ccno).HasColumnName("CCNo");

//                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

//                entity.Property(e => e.Cvvno).HasColumnName("CVVNo");

//                entity.Property(e => e.ExpiryMonth).HasMaxLength(10);

//                entity.Property(e => e.ExpiryYear).HasMaxLength(10);

//                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
//            });

//            modelBuilder.Entity<Payments>(entity =>
//            {
//                entity.Property(e => e.Amount).HasColumnType("decimal(18, 5)");

//                entity.Property(e => e.ExchangeRate).HasColumnType("decimal(18, 5)");

//                entity.Property(e => e.PaymentDate).HasColumnType("datetime");

//                entity.Property(e => e.Pgcurrency).HasColumnName("PGCurrency");

//                entity.HasOne(d => d.Appoinment)
//                    .WithMany(p => p.Payments)
//                    .HasForeignKey(d => d.AppoinmentId)
//                    .HasConstraintName("FK_Payments_Appoinments");
//            });

//            modelBuilder.Entity<PaymentsLog>(entity =>
//            {
//                entity.Property(e => e.Created).HasColumnType("datetime");

//                entity.Property(e => e.PaymentMethod).HasMaxLength(200);

//                entity.Property(e => e.Updated).HasColumnType("datetime");
//            });

//            modelBuilder.Entity<PharmaceuticalsCompany>(entity =>
//            {
//                entity.Property(e => e.ComanyName).HasMaxLength(256);

//                entity.Property(e => e.Email).HasMaxLength(256);

//                entity.Property(e => e.ProfileUrl).HasMaxLength(256);

//                entity.HasOne(d => d.User)
//                    .WithMany(p => p.PharmaceuticalsCompany)
//                    .HasForeignKey(d => d.UserId)
//                    .HasConstraintName("FK_PharmaceuticalsCompany_Users");
//            });

//            modelBuilder.Entity<PharmaCompanyRepMap>(entity =>
//            {
//                entity.Property(e => e.Created).HasColumnType("datetime");

//                entity.HasOne(d => d.PharmaCom)
//                    .WithMany(p => p.PharmaCompanyRepMap)
//                    .HasForeignKey(d => d.PharmaComId)
//                    .OnDelete(DeleteBehavior.ClientSetNull)
//                    .HasConstraintName("FK_PharmaCompanyRepMap_PharmaceuticalsCompany");

//                entity.HasOne(d => d.PharmaRep)
//                    .WithMany(p => p.PharmaCompanyRepMap)
//                    .HasForeignKey(d => d.PharmaRepId)
//                    .OnDelete(DeleteBehavior.ClientSetNull)
//                    .HasConstraintName("FK_PharmaCompanyRepMap_PharmaRepresentative");
//            });

//            modelBuilder.Entity<PharmaRepDocumentMap>(entity =>
//            {
//                entity.HasOne(d => d.Document)
//                    .WithMany(p => p.PharmaRepDocumentMap)
//                    .HasForeignKey(d => d.DocumentId)
//                    .HasConstraintName("FK_PharmaRepDocumentMap_Documents");
//            });

//            modelBuilder.Entity<PharmaRepresentative>(entity =>
//            {
//                entity.Property(e => e.CompanyName).HasMaxLength(250);

//                entity.Property(e => e.CompleteAddress).HasMaxLength(500);

//                entity.Property(e => e.ContractDate).HasColumnType("datetime");

//                entity.Property(e => e.ContractExpiryDate).HasColumnType("datetime");

//                entity.Property(e => e.FirstName).HasMaxLength(250);

//                entity.Property(e => e.ProfileUrl).HasMaxLength(150);

//                entity.Property(e => e.RegNumber).HasMaxLength(250);

//                entity.Property(e => e.SecondName).HasMaxLength(250);

//                entity.Property(e => e.ServicesHours).HasMaxLength(150);

//                entity.Property(e => e.ShortDescription).HasMaxLength(500);

//                entity.Property(e => e.TerityDefination).HasMaxLength(250);

//                entity.HasOne(d => d.User)
//                    .WithMany(p => p.PharmaRepresentative)
//                    .HasForeignKey(d => d.UserId)
//                    .OnDelete(DeleteBehavior.ClientSetNull)
//                    .HasConstraintName("FK_PharmaRepresentative_Users");
//            });

//            modelBuilder.Entity<PriceListMaster>(entity =>
//            {
//                entity.Property(e => e.Title).HasMaxLength(50);

//                entity.HasOne(d => d.Category)
//                    .WithMany(p => p.PriceListMaster)
//                    .HasForeignKey(d => d.CategoryId)
//                    .OnDelete(DeleteBehavior.ClientSetNull)
//                    .HasConstraintName("FK_PriceListMaster_DoctorCategoryType");
//            });

//            modelBuilder.Entity<SpecializationDetailsAndDiploma>(entity =>
//            {
//                entity.Property(e => e.NameofDegree).HasMaxLength(150);

//                entity.Property(e => e.Specialization).HasMaxLength(256);

//                entity.Property(e => e.University).HasMaxLength(256);
//            });

//            modelBuilder.Entity<SpecilizationCategory>(entity =>
//            {
//                entity.Property(e => e.HunName).HasMaxLength(250);

//                entity.Property(e => e.Name).HasMaxLength(250);
//            });

//            modelBuilder.Entity<TempDoctors>(entity =>
//            {
//                entity.Property(e => e.Country).HasMaxLength(50);

//                entity.Property(e => e.Dob)
//                    .HasColumnName("DOB")
//                    .HasColumnType("datetime");

//                entity.Property(e => e.FirstName).IsRequired();

//                entity.Property(e => e.Gander).HasMaxLength(50);

//                entity.Property(e => e.MedicalRegistrationNo)
//                    .IsRequired()
//                    .HasMaxLength(100);

//                entity.Property(e => e.PhoneNo).HasMaxLength(50);

//                entity.Property(e => e.PhotoUrl).HasColumnName("PhotoURL");

//                entity.Property(e => e.RegistrationNo).HasMaxLength(550);

//                entity.Property(e => e.State).HasMaxLength(50);

//                entity.Property(e => e.StreetNumber).HasMaxLength(50);

//                entity.Property(e => e.Zipcode)
//                    .HasMaxLength(20)
//                    .IsUnicode(false);
//            });

//            modelBuilder.Entity<TempNurses>(entity =>
//            {
//                entity.ToTable("Temp_Nurses");

//                entity.Property(e => e.City).HasMaxLength(150);

//                entity.Property(e => e.Country).HasMaxLength(150);

//                entity.Property(e => e.Gender).HasMaxLength(50);

//                entity.Property(e => e.MobileNo).HasMaxLength(50);

//                entity.Property(e => e.ProfileUrl).HasMaxLength(150);

//                entity.Property(e => e.ZipCode).HasMaxLength(150);
//            });

//            modelBuilder.Entity<Transfers>(entity =>
//            {
//                entity.Property(e => e.Amount).HasColumnType("decimal(18, 5)");

//                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

//                entity.Property(e => e.PaymentMethod).HasMaxLength(50);

//                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
//            });

//            modelBuilder.Entity<UserGroup>(entity =>
//            {
//                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

//                entity.Property(e => e.GroupName).IsRequired();
//            });

//            modelBuilder.Entity<Users>(entity =>
//            {
//                entity.Property(e => e.CreatedAt).HasColumnType("datetime");

//                entity.Property(e => e.DeletedAt).HasColumnType("datetime");

//                entity.Property(e => e.Email)
//                    .IsRequired()
//                    .HasMaxLength(256)
//                    .IsUnicode(false);

//                entity.Property(e => e.EmailOtp).HasColumnName("EmailOTP");

//                entity.Property(e => e.LastMeeting).HasColumnType("datetime");

//                entity.Property(e => e.Lastlogin).HasColumnType("datetime");

//                entity.Property(e => e.MobileOtp).HasColumnName("MobileOTP");

//                entity.Property(e => e.PhoneNo)
//                    .HasMaxLength(20)
//                    .IsUnicode(false);

//                entity.Property(e => e.UpdatedAt).HasColumnType("datetime");
//            });

//            modelBuilder.Entity<Workplace>(entity =>
//            {
//                entity.Property(e => e.EndDate).HasColumnType("datetime");

//                entity.Property(e => e.StartDate).HasColumnType("datetime");
//            });
//        }
//    }
//}
