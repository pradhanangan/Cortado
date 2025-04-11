CREATE TABLE "Titles"
(
    "Id" serial,
    "Code" text NOT NULL,
    "Description" text NOT NULL,
    "Comment" text NOT NULL,
    CONSTRAINT "PK_Titles" PRIMARY KEY("Id")
);

INSERT INTO "Titles" ("Id", "Code", "Description", "Comment") VALUES (1, 'MISS', 'Miss', '');
INSERT INTO "Titles" ("Id", "Code", "Description", "Comment") VALUES (2, 'MR', 'Mr', '');
INSERT INTO "Titles" ("Id", "Code", "Description", "Comment") VALUES (3, 'MRS', 'Mrs', '');
INSERT INTO "Titles" ("Id", "Code", "Description", "Comment") VALUES (4, 'MS', 'Ms', '');
INSERT INTO "Titles" ("Id", "Code", "Description", "Comment") VALUES (5, 'MASTER', 'Master', '');

CREATE TABLE "Genders"
(
    "Id" serial,
    "Code" text NOT NULL,
    "Description" text NOT NULL,
    "Comment" text NOT NULL,
    CONSTRAINT "PK_Genders" PRIMARY KEY("Id")
);

INSERT INTO "Genders" ("Id", "Code", "Description", "Comment") VALUES (1, 'F', 'Female', '');
INSERT INTO "Genders" ("Id", "Code", "Description", "Comment") VALUES (2, 'M', 'Male', '');
INSERT INTO "Genders" ("Id", "Code", "Description", "Comment") VALUES (3, 'O', 'Other Gender', '');

CREATE TABLE "Members"
(
    "Id" serial,
    "TitleId" bigint NOT NULL,
    "FirstName" text NOT NULL,
    "MiddleName" text NOT NULL,
    "LastName" text NOT NULL,
    "Email" text NOT NULL,
    "Phone" text NOT NULL,
    "BirthDate" date NOT NULL,
    "GenderId" bigint NOT NULL,
    "IsDeleted" boolean NOT NULL DEFAULT false,
    "CreatedBy" text NOT NULL,
    "CreatedDate" timestamp with time zone NOT NULL,
    "LastModifiedBy" text NOT NULL,
    "LastModifiedDate" timestamp with time zone,
     CONSTRAINT "PK_Members" PRIMARY KEY("Id"),
     CONSTRAINT "FK_Members_Titles_TitleId" FOREIGN KEY("TitleId") REFERENCES "Titles"("Id"),
     CONSTRAINT "FK_Members_Genders_GenderId" FOREIGN KEY("GenderId") REFERENCES "Genders"("Id"),
     CONSTRAINT "UK_Members_Email" UNIQUE("Email")
);

CREATE TABLE "Memberships"
(
    "Id" serial,
    "StartDate" date NOT NULL,
    "EndDate" date NOT NULL,
    "MemberId" bigint NOT NULL,
    "CreatedBy" text NOT NULL,
    "CreatedDate" timestamp with time zone NOT NULL,
    "LastModifiedBy" text NOT NULL,
    "LastModifiedDate" timestamp with time zone,
    CONSTRAINT "PK_Memberships" PRIMARY KEY("Id"),
    CONSTRAINT "FK_Memberships_Members_MemberId" FOREIGN KEY("MemberId") REFERENCES "Members"("Id") ON DELETE CASCADE
);

CREATE TABLE "Addresses"
(
    "Id" serial,
    "BuildingName" text NULL,
    "StreetAddress" text NOT NULL,
    "AdditionalStreetAddress" text NULL,
    "Suburb" text NULL,
    "TownOrCity" text NULL,
    "PostCode" text NULL,
    "Country" text NULL,
    "IsPrimary" boolean NOT NULL DEFAULT false,
    "MemberId" bigint NOT NULL,
    "CreatedBy" text NOT NULL,
    "CreatedDate" timestamp with time zone NOT NULL,
    "LastModifiedBy" text NOT NULL,
    "LastModifiedDate" timestamp with time zone,
    CONSTRAINT "PK_Addresses" PRIMARY KEY("Id"),
    CONSTRAINT "FK_Addresses_Members_MemberId" FOREIGN KEY("MemberId") REFERENCES "Members"("Id") ON DELETE CASCADE
);

CREATE TABLE "Activities"
(
    "Id" serial,
    "Code" text NOT NULL,
    "Description" text NOT NULL,
    "StartDate" date NOT NULL,
    "EndDate" date NOT NULL,
    "Duration" int NOT NULL,
    "CreatedBy" text NOT NULL,
    "CreatedDate" timestamp with time zone NOT NULL,
    "LastModifiedBy" text NOT NULL,
    "LastModifiedDate" timestamp with time zone,
    CONSTRAINT "PK_Activities" PRIMARY KEY("Id")
);

CREATE TABLE "Schedules"
(
    "Id" serial,
    "StartDate" timestamp with time zone NOT NULL,
    "EndDate" timestamp with time zone NOT NULL,
    "ActivityId" bigint NOT NULL,
    "CreatedBy" text NOT NULL,
    "CreatedDate" timestamp with time zone NOT NULL,
    "LastModifiedBy" text NOT NULL,
    "LastModifiedDate" timestamp with time zone,
    CONSTRAINT "PK_Schedules" PRIMARY KEY("Id"),
    CONSTRAINT "FK_Schedules_Activities_ActivityId" FOREIGN KEY("ActivityId") REFERENCES "Activities"("Id") ON DELETE CASCADE
);

CREATE TABLE "Registrations"
(
    "Id" serial,
    "MemberId" bigint NOT NULL,
    "ActivityId" bigint NOT NULL,
    "RegistrationDate" timestamp with time zone NOT NULL,
    "CreatedBy" text NOT NULL,
    "CreatedDate" timestamp with time zone NOT NULL,
    "LastModifiedBy" text NOT NULL,
    "LastModifiedDate" timestamp with time zone,
    CONSTRAINT "PK_Registrations" PRIMARY KEY("Id"),
    CONSTRAINT "FK_Registrations_Members_MemberId" FOREIGN KEY("MemberId") REFERENCES "Members"("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_Registrations_Activities_ActivityId" FOREIGN KEY("ActivityId") REFERENCES "Activities"("Id") ON DELETE CASCADE
);

CREATE TABLE "Users"
(
    "Id" serial,
    "IdentityId" uuid NOT NULL,
    "Username" text NOT NULL,
    "Email" text NOT NULL,
    "CreatedBy" text NOT NULL,
    "CreatedDate" timestamp with time zone NOT NULL,
    "LastModifiedBy" text NOT NULL,
    "LastModifiedDate" timestamp with time zone,
     CONSTRAINT "PK_Identities" PRIMARY KEY("Id")
);

CREATE TABLE "UserMembers"
(
	"Id" serial,
	"UserId" bigint NOT NULL,
	"MemberId" bigint NOT NULL,
	"CreatedBy" text NOT NULL,
	"CreatedDate" timestamp with time zone NOT NULL,
	"LastModifiedBy" text NOT NULL,
	"LastModifiedDate" timestamp with time zone,
	CONSTRAINT "PK_UserMembers" PRIMARY KEY("Id"),
	CONSTRAINT "FK_UserMembers_Users_UserId" FOREIGN KEY("UserId") REFERENCES "Users"("Id") ON DELETE CASCADE,
	CONSTRAINT "FK_UserMembers_Members_MemberId" FOREIGN KEY("MemberId") REFERENCES "Members"("Id") ON DELETE CASCADE
);


CREATE TABLE "Bookings"
(
	"Id" UUID,
	"ActivityId" bigint NOT NULL,
    "Email" text NOT NULL,
    "RequestedQuantity" int NOT NULL,
	"BookingDate" timestamp with time zone NOT NULL,
	"CreatedBy" text NOT NULL,
	"CreatedDate" timestamp with time zone NOT NULL,
	"LastModifiedBy" text NOT NULL,
	"LastModifiedDate" timestamp with time zone,
	CONSTRAINT "PK_Bookings" PRIMARY KEY("Id"),
	CONSTRAINT "FK_Bookings_Activities_ActivityId" FOREIGN KEY("ActivityId") REFERENCES "Activities"("Id") ON DELETE CASCADE,
);


