using System;
using System.Collections.Generic;
using System.Linq;
using CompetitionsTracking.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CompetitionsTracking.Infrastructure.Data
{
    public static class DatabaseSeeder
    {
        public static void SeedIfEmpty(CompetitionsTrackingDbContext context)
        {
            RepairCoachProfiles(context);

            if (context.Users.Any(u => u.Username == "coach"))
            {
                if (context.Results.Count() < 10) 
                {
                }
                else
                {
                    return;
                }
            }

            context.Database.ExecuteSqlRaw("DELETE FROM team_members");
            context.Appeals.RemoveRange(context.Appeals);
            context.Results.RemoveRange(context.Results);
            context.Scores.RemoveRange(context.Scores);
            context.Entries.RemoveRange(context.Entries);
            context.Judges.RemoveRange(context.Judges);
            context.Competitions.RemoveRange(context.Competitions);
            context.Disciplines.RemoveRange(context.Disciplines);
            context.Categories.RemoveRange(context.Categories);
            context.Apparatuses.RemoveRange(context.Apparatuses);
            context.Teams.RemoveRange(context.Teams);
            
            var personsWithMentors = context.Persons.Where(p => p.MentorId != null).ToList();
            foreach (var p in personsWithMentors) p.MentorId = null;
            context.SaveChanges();

            context.Participants.RemoveRange(context.Participants);
            context.Users.RemoveRange(context.Users);
            context.SaveChanges();

            var hasher = new PasswordHasher<User>();
            var rnd = new Random();

            var admin = new User { Username = "admin", Email = "admin@example.com", Role = UserRole.Admin, IsApproved = true, CreatedAt = DateTime.UtcNow };
            admin.PasswordHash = hasher.HashPassword(admin, "admin123");

            var myCoachUser = new User { Username = "coach", Email = "coach@gym.ua", Role = UserRole.Trainee, IsApproved = true, CreatedAt = DateTime.UtcNow };
            myCoachUser.PasswordHash = hasher.HashPassword(myCoachUser, "1.q.a.z.");

            var trainee = new User { Username = "trainee", Email = "trainee@example.com", Role = UserRole.Trainee, IsApproved = true, CreatedAt = DateTime.UtcNow };
            trainee.PasswordHash = hasher.HashPassword(trainee, "trainee123");

            context.Users.AddRange(admin, myCoachUser, trainee);

            var coachMy = new Person { Name = "Олександр", Surname = "Тренерко", Country = "Україна", DateOfBirth = new DateTime(1980, 5, 10).ToUniversalTime(), Gender = Gender.Male, Type = "Person" };
            var coachDer = new Person { Name = "Ірина", Surname = "Дерюгіна", Country = "Україна", DateOfBirth = new DateTime(1958, 1, 11).ToUniversalTime(), Gender = Gender.Female, Type = "Person" };
            var coachVyn = new Person { Name = "Ганна", Surname = "Вінницька", Country = "Україна", DateOfBirth = new DateTime(1985, 8, 20).ToUniversalTime(), Gender = Gender.Female, Type = "Person" };
            
            myCoachUser.Person = coachMy;
            trainee.Person = coachDer;

            context.Persons.AddRange(coachMy, coachDer, coachVyn);
            context.SaveChanges();

            var athletesMy = new List<Person>
            {
                new Person { Name = "Марія", Surname = "Петренко", Country = "Україна", DateOfBirth = new DateTime(2010, 3, 15).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachMy },
                new Person { Name = "Анастасія", Surname = "Сидоренко", Country = "Україна", DateOfBirth = new DateTime(2011, 7, 22).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachMy },
                new Person { Name = "Дар'я", Surname = "Ковальчук", Country = "Україна", DateOfBirth = new DateTime(2009, 12, 10).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachMy },
                new Person { Name = "Вікторія", Surname = "Лисенко", Country = "Україна", DateOfBirth = new DateTime(2012, 1, 5).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachMy },
                new Person { Name = "Єлизавета", Surname = "Мороз", Country = "Україна", DateOfBirth = new DateTime(2010, 11, 30).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachMy }
            };

            var athletesDer = new List<Person>
            {
                new Person { Name = "Влада", Surname = "Нікольченко", Country = "Україна", DateOfBirth = new DateTime(2008, 12, 9).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachDer },
                new Person { Name = "Вікторія", Surname = "Онопрієнко", Country = "Україна", DateOfBirth = new DateTime(2007, 10, 18).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachDer }
            };

            context.Persons.AddRange(athletesMy);
            context.Persons.AddRange(athletesDer);
            context.SaveChanges();

            var teamMy = new Team { Name = "Київська Грація", Coach = coachMy, Members = athletesMy, Type = "Team" };
            var teamDer = new Team { Name = "Школа Дерюгіних", Coach = coachDer, Members = athletesDer, Type = "Team" };
            
            context.Teams.AddRange(teamMy, teamDer);
            context.SaveChanges();

            var appHoop = new Apparatus { Type = "Обруч" };
            var appBall = new Apparatus { Type = "М'яч" };
            var appClubs = new Apparatus { Type = "Булави" };
            var appRibbon = new Apparatus { Type = "Стрічка" };
            context.Apparatuses.AddRange(appHoop, appBall, appClubs, appRibbon);

            var discHoop = new Discipline { Type = "Індивідуальна (Обруч)", Apparatus = appHoop };
            var discBall = new Discipline { Type = "Індивідуальна (М'яч)", Apparatus = appBall };
            var discGroupBall = new Discipline { Type = "Групова (5 м'ячів)", Apparatus = appBall };
            var discGroupClubs = new Discipline { Type = "Групова (3 стрічки + 2 м'ячі)", Apparatus = appRibbon };
            context.Disciplines.AddRange(discHoop, discBall, discGroupBall, discGroupClubs);

            var catSeniors = new Category { Type = "Сеньйорки", MinAge = 16, MaxAge = 99 };
            var catJuniors = new Category { Type = "Юніорки", MinAge = 13, MaxAge = 15 };
            var catHopes = new Category { Type = "Надії", MinAge = 10, MaxAge = 12 };
            context.Categories.AddRange(catSeniors, catJuniors, catHopes);
            context.SaveChanges();

            var compPast = new Competition { Title = "Зимовий Кубок України 2026", City = "Львів", Country = "Україна", Level = CompetitionLevel.National, StartDate = DateTime.UtcNow.AddMonths(-2), EndDate = DateTime.UtcNow.AddMonths(-2).AddDays(3), Status = CompetitionStatus.Finished };
            var compNow = new Competition { Title = "Весняні Грації 2026", City = "Київ", Country = "Україна", Level = CompetitionLevel.Local, StartDate = DateTime.UtcNow.AddDays(-2), EndDate = DateTime.UtcNow.AddDays(2), Status = CompetitionStatus.Ongoing };
            var compFuture = new Competition { Title = "Чемпіонат Європи 2026", City = "Будапешт", Country = "Угорщина", Level = CompetitionLevel.International, StartDate = DateTime.UtcNow.AddMonths(1), EndDate = DateTime.UtcNow.AddMonths(1).AddDays(5), Status = CompetitionStatus.Planned };
            
            context.Competitions.AddRange(compPast, compNow, compFuture);
            context.SaveChanges();

            var judges = new List<Judge>();
            string[] judgeNames = { "Тетяна", "Олена", "Марина", "Світлана", "Оксана" };
            foreach (var name in judgeNames)
            {
                var jp = new Person { Name = name, Surname = "Судденко", Country = "Україна", Type = "Person" };
                context.Persons.Add(jp);
                var j = new Judge { Person = jp, QualificationLevel = "Міжнародна" };
                judges.Add(j);
                context.Judges.Add(j);
            }
            context.SaveChanges();

            var pastEntries = new List<Entry>();
            foreach (var athlete in athletesMy.Concat(athletesDer))
            {
                var entry = new Entry 
                { 
                    Competition = compPast, 
                    Participant = athlete, 
                    Discipline = discHoop, 
                    Category = athlete.DateOfBirth.Year > 2010 ? catHopes : catJuniors, 
                    ApplicationStatus = ApplicationStatus.Accepted, 
                    EntryStatus = EntryStatus.Finished, 
                    SubmittedAt = DateTime.UtcNow.AddMonths(-3) 
                };
                pastEntries.Add(entry);
            }
            var teamPastEntries = new List<Entry>
            {
                new Entry { Competition = compPast, Participant = teamMy, Discipline = discGroupBall, Category = catJuniors, ApplicationStatus = ApplicationStatus.Accepted, EntryStatus = EntryStatus.Finished, SubmittedAt = DateTime.UtcNow.AddMonths(-3) },
                new Entry { Competition = compPast, Participant = teamDer, Discipline = discGroupBall, Category = catJuniors, ApplicationStatus = ApplicationStatus.Accepted, EntryStatus = EntryStatus.Finished, SubmittedAt = DateTime.UtcNow.AddMonths(-3) }
            };
            context.Entries.AddRange(pastEntries);
            context.Entries.AddRange(teamPastEntries);
            context.SaveChanges();

            var allPast = pastEntries.Concat(teamPastEntries).ToList();
            foreach (var entry in allPast)
            {
                float d = 10.0f + (float)rnd.NextDouble() * 5.0f;
                float a = 7.0f + (float)rnd.NextDouble() * 2.0f;
                float e = 7.0f + (float)rnd.NextDouble() * 2.0f;

                context.Scores.Add(new Score { Entry = entry, Judge = judges[0], Type = ScoreType.D, ScoreValue = d });
                context.Scores.Add(new Score { Entry = entry, Judge = judges[1], Type = ScoreType.A, ScoreValue = a });
                context.Scores.Add(new Score { Entry = entry, Judge = judges[2], Type = ScoreType.E, ScoreValue = e });

                float total = d + a + e;
                context.Results.Add(new Result { Entry = entry, Place = 0, FinalScore = total }); // Place will be updated below
            }
            context.SaveChanges();

            var pastResults = context.Results.Where(r => r.Entry.CompetitionId == compPast.Id).Include(r => r.Entry).ToList();
            var sortedIndiv = pastResults.Where(r => r.Entry.Participant is Person).OrderByDescending(r => r.FinalScore).ToList();
            for (int i = 0; i < sortedIndiv.Count; i++)
            {
                sortedIndiv[i].Place = i + 1;
                if (i == 0) sortedIndiv[i].AwardedMedal = "Золото";
                else if (i == 1) sortedIndiv[i].AwardedMedal = "Срібло";
                else if (i == 2) sortedIndiv[i].AwardedMedal = "Бронза";
            }
            var sortedTeams = pastResults.Where(r => r.Entry.Participant is Team).OrderByDescending(r => r.FinalScore).ToList();
            for (int i = 0; i < sortedTeams.Count; i++)
            {
                sortedTeams[i].Place = i + 1;
                if (i == 0) sortedTeams[i].AwardedMedal = "Золото";
            }
            context.SaveChanges();

            var ongoingEntries = new List<Entry>();
            foreach (var athlete in athletesMy)
            {
                ongoingEntries.Add(new Entry 
                { 
                    Competition = compNow, 
                    Participant = athlete, 
                    Discipline = discBall, 
                    Category = athlete.DateOfBirth.Year > 2010 ? catHopes : catJuniors, 
                    ApplicationStatus = ApplicationStatus.Accepted, 
                    EntryStatus = EntryStatus.Active, 
                    SubmittedAt = DateTime.UtcNow.AddDays(-10) 
                });
            }
            foreach (var athlete in athletesDer)
            {
                ongoingEntries.Add(new Entry 
                { 
                    Competition = compNow, 
                    Participant = athlete, 
                    Discipline = discBall, 
                    Category = catJuniors, 
                    ApplicationStatus = ApplicationStatus.Accepted, 
                    EntryStatus = EntryStatus.Active, 
                    SubmittedAt = DateTime.UtcNow.AddDays(-10) 
                });
            }
            context.Entries.AddRange(ongoingEntries);
            context.SaveChanges();

            foreach (var entry in ongoingEntries.Take(3))
            {
                context.Scores.Add(new Score { Entry = entry, Judge = judges[0], Type = ScoreType.D, ScoreValue = 12.5f + (float)rnd.NextDouble() });
                context.Scores.Add(new Score { Entry = entry, Judge = judges[1], Type = ScoreType.A, ScoreValue = 8.2f });
            }
            context.SaveChanges();

            var coachResult = pastResults.FirstOrDefault(r => r.Entry.ParticipantId == athletesMy[0].Id);
            if (coachResult != null)
            {
                context.Appeals.Add(new Appeal 
                { 
                    Result = coachResult, 
                    Reason = "Оцінка за складність тіла була занижена на 0.3 бала. Просимо переглянути відеоповтор елементу 'панше'.", 
                    Status = AppealStatus.Pending, 
                    CreatedAt = DateTime.UtcNow.AddDays(-1) 
                });

                var otherResult = pastResults.FirstOrDefault(r => r.Entry.ParticipantId == athletesMy[1].Id);
                if (otherResult != null)
                {
                    context.Appeals.Add(new Appeal 
                    { 
                        Result = otherResult, 
                        Reason = "Технічна помилка при введенні оцінки E. Замість 8.5 введено 8.0.", 
                        Status = AppealStatus.Approved, 
                        CreatedAt = DateTime.UtcNow.AddDays(-5),
                        ResolvedAt = DateTime.UtcNow.AddDays(-4)
                    });
                }
            }

            context.SaveChanges();
        }

        private static void RepairCoachProfiles(CompetitionsTrackingDbContext context)
        {
            var coachesWithoutProfiles = context.Users
                .Where(u => u.Role == UserRole.Trainee && u.PersonId == null)
                .ToList();

            if (!coachesWithoutProfiles.Any()) return;

            foreach (var user in coachesWithoutProfiles)
            {
                var person = new Person
                {
                    Name = user.Username,
                    Surname = "(Auto-repaired)",
                    Country = "Україна",
                    DateOfBirth = DateTime.UtcNow.AddYears(-20),
                    Gender = Gender.Female,
                    Type = "Person"
                };
                context.Persons.Add(person);
                user.Person = person;
            }
            context.SaveChanges();
        }
    }
}
