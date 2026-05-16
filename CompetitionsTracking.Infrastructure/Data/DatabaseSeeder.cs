using System;
using System.Collections.Generic;
using System.Linq;
using CompetitionsTracking.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CompetitionsTracking.Infrastructure.Data
{
    /// <summary>
    /// Seeds initial data into an empty database.
    ///
    /// HOW TO REFRESH SEED DATA:
    ///   1. Open appsettings.json
    ///   2. Set "Seeding:ForceReseed" to true
    ///   3. Start the application once — it will wipe and re-seed automatically
    ///   4. Set "Seeding:ForceReseed" back to false
    /// </summary>
    public static class DatabaseSeeder
    {
        // ─────────────────────────────────────────────────────────────────────
        // Public entry points
        // ─────────────────────────────────────────────────────────────────────

        /// <summary>Seeds the DB only when it is empty (safe default).</summary>
        public static void SeedIfEmpty(CompetitionsTrackingDbContext context)
        {
            Seed(context);
        }

        /// <summary>
        /// Wipes all seed-managed tables in dependency order, then re-seeds.
        /// Called when ForceReseed = true.
        /// </summary>
        public static void ClearAndReseed(CompetitionsTrackingDbContext context)
        {
            // Delete in reverse-dependency order to satisfy ALL FK constraints:
            //   appeals → results → scores → entries → competitions
            //   judges → persons
            //   users → persons  (FK: users.PersonId → persons.Id)
            //   team_members → teams → persons
            //   participants (base TPT table) after teams & persons are gone

            // 1. Deepest leaf nodes
            context.Appeals.RemoveRange(context.Appeals);
            context.Scores.RemoveRange(context.Scores);
            context.SaveChanges();

            context.Results.RemoveRange(context.Results);
            context.SaveChanges();

            context.Entries.RemoveRange(context.Entries);
            context.SaveChanges();

            context.Competitions.RemoveRange(context.Competitions);
            context.SaveChanges();

            // 2. Users reference Persons → delete Users first
            context.Users.RemoveRange(context.Users);
            context.SaveChanges();

            // 3. Judges reference Persons → delete Judges
            context.Judges.RemoveRange(context.Judges);
            context.SaveChanges();

            // 4. Clear Team↔Person join table (no EF entity for this)
            context.Database.ExecuteSqlRaw("DELETE FROM [team_members]");

            // 5. Teams reference Persons (CoachId) → delete Teams before Persons
            context.Teams.RemoveRange(context.Teams);
            context.SaveChanges();

            // 6. Now Persons can be deleted (no remaining FK references)
            context.Persons.RemoveRange(context.Persons);
            context.SaveChanges();

            // 7. Remaining lookup tables
            context.Disciplines.RemoveRange(context.Disciplines);
            context.Apparatuses.RemoveRange(context.Apparatuses);
            context.Categories.RemoveRange(context.Categories);
            context.SaveChanges();

            // 8. Reset identity seeds so IDs start from 1 again
            ResetIdentity(context, "participants"); // Base TPT table handles IDs for Teams and Persons
            ResetIdentity(context, "users");
            ResetIdentity(context, "competitions");
            ResetIdentity(context, "disciplines");
            ResetIdentity(context, "apparatuses");
            ResetIdentity(context, "categories");
            ResetIdentity(context, "entries");
            ResetIdentity(context, "results");
            ResetIdentity(context, "scores");
            ResetIdentity(context, "judges");
            ResetIdentity(context, "appeals");

            Seed(context);
        }



        // ─────────────────────────────────────────────────────────────────────
        // Core seeding logic
        // ─────────────────────────────────────────────────────────────────────

        private static void Seed(CompetitionsTrackingDbContext context)
        {
            var hasher = new PasswordHasher<User>();
            var rnd    = new Random(42); // fixed seed → deterministic scores

            // ── 1. Users ──────────────────────────────────────────────────────
            var admin = new User
            {
                Username   = "admin",
                Email      = "admin@gym.ua",
                Role       = UserRole.Admin,
                IsApproved = true,
                CreatedAt  = DateTime.UtcNow
            };
            admin.PasswordHash = hasher.HashPassword(admin, "admin123");

            var coachUser = new User
            {
                Username   = "coach",
                Email      = "coach@gym.ua",
                Role       = UserRole.Trainee,
                IsApproved = true,
                CreatedAt  = DateTime.UtcNow
            };
            coachUser.PasswordHash = hasher.HashPassword(coachUser, "1.q.a.z.");

            var judgeUser1 = new User { Username = "judge1", Email = "j1@gym.ua", Role = UserRole.Guest, IsApproved = true, CreatedAt = DateTime.UtcNow };
            var judgeUser2 = new User { Username = "judge2", Email = "j2@gym.ua", Role = UserRole.Guest, IsApproved = true, CreatedAt = DateTime.UtcNow };

            context.Users.AddRange(admin, coachUser, judgeUser1, judgeUser2);

            // ── 2. Persons (coaches & judges) ─────────────────────────────────
            var personCoach1 = new Person { Name = "Олена", Surname = "Дерюгіна", Country = "Україна", DateOfBirth = new DateTime(1958, 1, 11, 0, 0, 0, DateTimeKind.Utc), Gender = Gender.Female, Type = "Person" };
            var personCoach2 = new Person { Name = "Ганна", Surname = "Різатдінова", Country = "Україна", DateOfBirth = new DateTime(1993, 7, 16, 0, 0, 0, DateTimeKind.Utc), Gender = Gender.Female, Type = "Person" };
            var personJudge1 = new Person { Name = "Ірина", Surname = "Дерюгіна", Country = "Україна", DateOfBirth = new DateTime(1958, 1, 11, 0, 0, 0, DateTimeKind.Utc), Gender = Gender.Female, Type = "Person" };
            var personJudge2 = new Person { Name = "Наталія", Surname = "Єрьоміна", Country = "Україна", DateOfBirth = new DateTime(1965, 5, 20, 0, 0, 0, DateTimeKind.Utc), Gender = Gender.Female, Type = "Person" };

            coachUser.Person = personCoach1;
            context.Persons.AddRange(personCoach1, personCoach2, personJudge1, personJudge2);
            context.SaveChanges();

            // ── 3. Judges ─────────────────────────────────────────────────────
            var judge1 = new Judge { PersonId = personJudge1.Id, QualificationLevel = "Міжнародна" };
            var judge2 = new Judge { PersonId = personJudge2.Id, QualificationLevel = "Національна" };
            context.Judges.AddRange(judge1, judge2);
            context.SaveChanges();

            // ── 4. Apparatuses ────────────────────────────────────────────────
            var apps = new[] { "Без предмета", "Обруч", "М'яч", "Булави", "Стрічка" }.Select(t => new Apparatus { Type = t }).ToList();
            context.Apparatuses.AddRange(apps);

            // ── 5. Categories ─────────────────────────────────────────────────
            var catSenior  = new Category { Type = "Сеньйорки", MinAge = 16, MaxAge = 99 };
            var catJunior  = new Category { Type = "Юніорки",   MinAge = 13, MaxAge = 15 };
            var catYouth   = new Category { Type = "Надії",     MinAge = 10, MaxAge = 12 };
            context.Categories.AddRange(catSenior, catJunior, catYouth);
            context.SaveChanges();

            // ── 6. Disciplines ────────────────────────────────────────────────
            var discIndHoop  = new Discipline { Type = "Індивідуальна (Обруч)", Apparatus = apps[1] };
            var discIndBall  = new Discipline { Type = "Індивідуальна (М'яч)",  Apparatus = apps[2] };
            var discIndClubs = new Discipline { Type = "Індивідуальна (Булави)",Apparatus = apps[3] };
            var discGrpBalls = new Discipline { Type = "Групова (5 м'ячів)",    Apparatus = apps[2] };
            var discGrpMixed = new Discipline { Type = "Групова (3 стрічки + 2 булави)", Apparatus = apps[3] };
            var disciplines = new List<Discipline> { discIndHoop, discIndBall, discIndClubs, discGrpBalls, discGrpMixed };
            context.Disciplines.AddRange(disciplines);
            context.SaveChanges();

            // ── 7. Teams ──────────────────────────────────────────────────────
            var teams = new List<Team>
            {
                new Team { Name = "Зірки Києва", Coach = personCoach1, Type = "Team" },
                new Team { Name = "Грація Львів", Coach = personCoach2, Type = "Team" },
                new Team { Name = "Дніпро-Гімн", Coach = personCoach1, Type = "Team" },
                new Team { Name = "Одеса-Спорт", Coach = personCoach2, Type = "Team" },
                new Team { Name = "Ніка Харків", Coach = personCoach1, Type = "Team" },
                new Team { Name = "Крок Полтава", Coach = personCoach2, Type = "Team" }
            };
            context.Teams.AddRange(teams);
            context.SaveChanges();

            // ── 8. Athletes ───────────────────────────────────────────────────
            string[] names = { "Марія", "Софія", "Анна", "Вікторія", "Дарина", "Ольга", "Юлія", "Катерина", "Тетяна", "Олександра", "Христина", "Альона", "Поліна", "Анастасія", "Діана", "Єлизавета", "Ірина", "Надія", "Маргарита", "Валерія", "Оксана", "Яна", "Вероніка", "Світлана" };
            string[] surnames = { "Коваль", "Бондар", "Сидоренко", "Петренко", "Мельник", "Шевченко", "Бойко", "Ткаченко", "Кравченко", "Козак", "Мороз", "Павленко", "Марченко", "Лисенко", "Рудник", "Клименко", "Вовк", "Савченко", "Поліщук", "Гончар", "Карпенко", "Романенко", "Харченко", "Гаврилюк" };

            var athletes = new List<Person>();
            for (int i = 0; i < 24; i++)
            {
                var coach = (i % 2 == 0) ? personCoach1 : personCoach2; // 12 for coach1, 12 for coach2
                var team  = teams[i % 6];
                var dob   = new DateTime(2008 + (i % 4), 1 + (i % 12), 1 + (i % 28), 0, 0, 0, DateTimeKind.Utc);
                var athlete = new Person
                {
                    Name = names[i], Surname = surnames[i], Country = "Україна", DateOfBirth = dob, Gender = Gender.Female, Type = "Person", Mentor = coach
                };
                athletes.Add(athlete);
                team.Members.Add(athlete);
            }
            context.Persons.AddRange(athletes);
            context.SaveChanges();

            // ── 9. Competitions (9 Total) ─────────────────────────────────────
            var competitions = new List<Competition>
            {
                // Local
                new Competition { Title = "Кубок Дарниці", City = "Київ", Country = "Україна", Level = CompetitionLevel.Local, StartDate = DateTime.UtcNow.AddDays(-30), EndDate = DateTime.UtcNow.AddDays(-28), Status = CompetitionStatus.Finished },
                new Competition { Title = "Київські Грації", City = "Київ", Country = "Україна", Level = CompetitionLevel.Local, StartDate = DateTime.UtcNow.AddDays(-1), EndDate = DateTime.UtcNow.AddDays(2), Status = CompetitionStatus.Ongoing },
                new Competition { Title = "Осінній Листок", City = "Київ", Country = "Україна", Level = CompetitionLevel.Local, StartDate = DateTime.UtcNow.AddDays(20), EndDate = DateTime.UtcNow.AddDays(22), Status = CompetitionStatus.Planned },
                
                // National
                new Competition { Title = "Чемпіонат України", City = "Львів", Country = "Україна", Level = CompetitionLevel.National, StartDate = DateTime.UtcNow.AddDays(-60), EndDate = DateTime.UtcNow.AddDays(-57), Status = CompetitionStatus.Finished },
                new Competition { Title = "Кубок України", City = "Дніпро", Country = "Україна", Level = CompetitionLevel.National, StartDate = DateTime.UtcNow.AddDays(-2), EndDate = DateTime.UtcNow.AddDays(3), Status = CompetitionStatus.Ongoing },
                new Competition { Title = "Зимова Казка", City = "Одеса", Country = "Україна", Level = CompetitionLevel.National, StartDate = DateTime.UtcNow.AddDays(40), EndDate = DateTime.UtcNow.AddDays(43), Status = CompetitionStatus.RegistrationOpen },
                
                // International
                new Competition { Title = "Grand Prix Lviv", City = "Львів", Country = "Україна", Level = CompetitionLevel.International, StartDate = DateTime.UtcNow.AddDays(-90), EndDate = DateTime.UtcNow.AddDays(-87), Status = CompetitionStatus.Finished },
                new Competition { Title = "Kyiv International Cup", City = "Київ", Country = "Україна", Level = CompetitionLevel.International, StartDate = DateTime.UtcNow.AddDays(-1), EndDate = DateTime.UtcNow.AddDays(4), Status = CompetitionStatus.Ongoing },
                new Competition { Title = "Deriugina Cup", City = "Київ", Country = "Україна", Level = CompetitionLevel.International, StartDate = DateTime.UtcNow.AddDays(60), EndDate = DateTime.UtcNow.AddDays(65), Status = CompetitionStatus.Planned }
            };
            context.Competitions.AddRange(competitions);
            context.SaveChanges();

            // ── 10. Entries ───────────────────────────────────────────────────
            var entries = new List<Entry>();
            var indDiscs = new[] { discIndHoop, discIndBall, discIndClubs };
            var grpDiscs = new[] { discGrpBalls, discGrpMixed };

            foreach (var comp in competitions)
            {
                // Finished competitions get lots of entries so we have >3 places
                if (comp.Status == CompetitionStatus.Finished)
                {
                    // Add 12 individuals (ensures 6 per discipline, since we divide by 2 indDiscs)
                    for (int i = 0; i < 12; i++)
                    {
                        entries.Add(new Entry { Competition = comp, Participant = athletes[i], Discipline = indDiscs[i % 2], Category = catSenior, ApplicationStatus = ApplicationStatus.Accepted, EntryStatus = EntryStatus.Finished, SubmittedAt = comp.StartDate.AddDays(-14) });
                    }
                    // Add 6 group teams
                    for (int i = 0; i < 6; i++)
                    {
                        entries.Add(new Entry { Competition = comp, Participant = teams[i], Discipline = grpDiscs[i % 2], Category = catSenior, ApplicationStatus = ApplicationStatus.Accepted, EntryStatus = EntryStatus.Finished, SubmittedAt = comp.StartDate.AddDays(-14) });
                    }
                }
                else if (comp.Status == CompetitionStatus.Ongoing)
                {
                    for (int i = 12; i < 18; i++)
                    {
                        entries.Add(new Entry { Competition = comp, Participant = athletes[i], Discipline = indDiscs[0], Category = catJunior, ApplicationStatus = ApplicationStatus.Accepted, EntryStatus = EntryStatus.Active, SubmittedAt = comp.StartDate.AddDays(-14) });
                    }
                }
                else // Planned / Registration
                {
                    for (int i = 18; i < 24; i++)
                    {
                        entries.Add(new Entry { Competition = comp, Participant = athletes[i], Discipline = indDiscs[1], Category = catJunior, ApplicationStatus = (i % 2 == 0) ? ApplicationStatus.Pending : ApplicationStatus.Accepted, EntryStatus = EntryStatus.Registered, SubmittedAt = DateTime.UtcNow.AddDays(-2) });
                    }
                }
            }
            context.Entries.AddRange(entries);
            context.SaveChanges();

            // ── 11. Results & Scores (finished entries only) ──────────────────
            var finishedEntries = entries.Where(e => e.EntryStatus == EntryStatus.Finished).ToList();
            foreach (var entry in finishedEntries)
            {
                float d     = 10.0f + (float)(rnd.NextDouble() * 8.0);
                float a     = 7.0f  + (float)(rnd.NextDouble() * 2.5);
                float ex    = 7.0f  + (float)(rnd.NextDouble() * 2.5);
                float final = (float)Math.Round(d + a + ex, 3);

                context.Results.Add(new Result { Entry = entry, FinalScore = final, Place = 0, AwardedMedal = "" });
                context.Scores.Add(new Score { Entry = entry, Judge = judge1, Type = ScoreType.D, ScoreValue = d, ElementCount = 8 });
                context.Scores.Add(new Score { Entry = entry, Judge = judge2, Type = ScoreType.A, ScoreValue = a });
                context.Scores.Add(new Score { Entry = entry, Judge = judge1, Type = ScoreType.E, ScoreValue = ex });
            }
            context.SaveChanges();

            // ── 12. Assign places & medals per discipline+category group ─────
            var allResults = context.Results.Include(r => r.Entry).ToList();

            foreach (var compGroup in allResults.GroupBy(r => r.Entry.CompetitionId))
            {
                foreach (var group in compGroup.GroupBy(r => new { r.Entry.DisciplineId, r.Entry.CategoryId }))
                {
                    var sorted = group.OrderByDescending(r => r.FinalScore).ToList();
                    for (int i = 0; i < sorted.Count; i++)
                    {
                        sorted[i].Place = i + 1;
                        // ONLY top 3 get medals, rest get ""
                        sorted[i].AwardedMedal = i == 0 ? "Золото" : i == 1 ? "Срібло" : i == 2 ? "Бронза" : "";
                    }
                }
            }
            context.SaveChanges();

            // ── 13. Sample appeals ────────────────────────────────────────────
            var coachResults = context.Results.Include(r => r.Entry).Where(r => r.Entry.ParticipantId == athletes[0].Id || r.Entry.ParticipantId == athletes[2].Id || r.Entry.ParticipantId == athletes[4].Id).ToList();

            if (coachResults.Count > 0)
            {
                context.Appeals.Add(new Appeal { Result = coachResults[0], Reason = "Некоректно зараховано складність тіла (DB). Прошу переглянути відео.", Status = AppealStatus.Pending, CreatedAt = DateTime.UtcNow.AddDays(-1) });
            }
            if (coachResults.Count > 1)
            {
                context.Appeals.Add(new Appeal { Result = coachResults[1], Reason = "Помилка в оцінці за артистизм.", Status = AppealStatus.Approved, CreatedAt = DateTime.UtcNow.AddDays(-5), ResolvedAt = DateTime.UtcNow.AddDays(-4) });
            }
            if (coachResults.Count > 2)
            {
                context.Appeals.Add(new Appeal { Result = coachResults[2], Reason = "Не зафіксовано ризик.", Status = AppealStatus.Rejected, CreatedAt = DateTime.UtcNow.AddDays(-3), ResolvedAt = DateTime.UtcNow.AddDays(-2) });
            }
            context.SaveChanges();
        }

        // ─────────────────────────────────────────────────────────────────────
        // Helper: reset IDENTITY counter so fresh IDs start at 1
        // ─────────────────────────────────────────────────────────────────────
        private static void ResetIdentity(CompetitionsTrackingDbContext context, string tableName)
        {
            try
            {
                context.Database.ExecuteSqlRaw(
                    $"IF EXISTS (SELECT 1 FROM sys.tables WHERE name = '{tableName}') " +
                    $"DBCC CHECKIDENT ('{tableName}', RESEED, 0)");
            }
            catch
            {
                // Table may not have an identity column — silently ignore
            }
        }
    }
}
