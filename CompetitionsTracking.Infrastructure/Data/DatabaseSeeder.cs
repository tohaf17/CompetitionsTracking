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

            if (context.Users.Any() || context.Competitions.Any() || context.Judges.Any())
            {
                EnsureCompetitionTypeSamples(context);
                EnsureAtLeastOneFinishedPerLevel(context);
                EnsureResultsForFinishedCompetitions(context);
                return;
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

            var admin = new User { Username = "admin", Email = "admin@example.com", Role = UserRole.Admin, IsApproved = true, CreatedAt = DateTime.UtcNow };
            admin.PasswordHash = hasher.HashPassword(admin, "admin123");

            var trainee = new User { Username = "trainee", Email = "trainee@example.com", Role = UserRole.Trainee, IsApproved = true, CreatedAt = DateTime.UtcNow };
            trainee.PasswordHash = hasher.HashPassword(trainee, "trainee123");

            var guest = new User { Username = "guest", Email = "guest@example.com", Role = UserRole.Guest, IsApproved = true, CreatedAt = DateTime.UtcNow };
            guest.PasswordHash = hasher.HashPassword(guest, "guest123");

            context.Users.AddRange(admin, trainee, guest);

            var coachKyiv = new Person { Name = "Олена", Surname = "Дерюгіна", Country = "Україна", DateOfBirth = new DateTime(1958, 2, 11).ToUniversalTime(), Gender = Gender.Female, Type = "Person" };
            var coachLviv = new Person { Name = "Альона", Surname = "Петренко", Country = "Україна", DateOfBirth = new DateTime(1985, 5, 20).ToUniversalTime(), Gender = Gender.Female, Type = "Person" };
            var coachKharkiv = new Person { Name = "Світлана", Surname = "Медведєва", Country = "Україна", DateOfBirth = new DateTime(1980, 10, 5).ToUniversalTime(), Gender = Gender.Female, Type = "Person" };
            var coachOdesa = new Person { Name = "Наталія", Surname = "Горбань", Country = "Україна", DateOfBirth = new DateTime(1978, 3, 12).ToUniversalTime(), Gender = Gender.Female, Type = "Person" };
            var coachDnipro = new Person { Name = "Ірина", Surname = "Савченко", Country = "Україна", DateOfBirth = new DateTime(1982, 11, 30).ToUniversalTime(), Gender = Gender.Female, Type = "Person" };

            trainee.Person = coachKyiv;

            context.Persons.AddRange(coachKyiv, coachLviv, coachKharkiv, coachOdesa, coachDnipro);

            var athleteK1 = new Person { Name = "Марія", Surname = "Коваль", Country = "Україна", DateOfBirth = new DateTime(2008, 3, 15).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachKyiv };
            var athleteK2 = new Person { Name = "Софія", Surname = "Бондар", Country = "Україна", DateOfBirth = new DateTime(2009, 7, 22).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachKyiv };
            var athleteK3 = new Person { Name = "Поліна", Surname = "Шевченко", Country = "Україна", DateOfBirth = new DateTime(2010, 1, 10).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachKyiv };
            var athleteK4 = new Person { Name = "Анна", Surname = "Лисенко", Country = "Україна", DateOfBirth = new DateTime(2011, 11, 3).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachKyiv };

            var athleteL1 = new Person { Name = "Вікторія", Surname = "Мельник", Country = "Україна", DateOfBirth = new DateTime(2012, 6, 18).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachLviv };
            var athleteL2 = new Person { Name = "Катерина", Surname = "Ткаченко", Country = "Україна", DateOfBirth = new DateTime(2007, 9, 25).ToUniversalTime(), Gender = Gender.Female, Type = "Person", Mentor = coachLviv };
            
            var allAthletes = new List<Person> { athleteK1, athleteK2, athleteK3, athleteK4, athleteL1, athleteL2 };
            context.Persons.AddRange(allAthletes);

            var teamKyiv = new Team { Name = "Зірки Києва", Coach = coachKyiv, Members = new List<Person> { athleteK1, athleteK2 }, Type = "Team" };
            var teamLviv = new Team { Name = "Грація Львів", Coach = coachLviv, Members = new List<Person> { athleteL1, athleteL2 }, Type = "Team" };

            context.Teams.AddRange(teamKyiv, teamLviv);

            var appHoop = new Apparatus { Type = "Обруч" };
            var appBall = new Apparatus { Type = "М'яч" };
            context.Apparatuses.AddRange(appHoop, appBall);

            var discHoop = new Discipline { Type = "Індивідуальна вправа (Обруч)", Apparatus = appHoop };
            var discBall = new Discipline { Type = "Індивідуальна вправа (М'яч)", Apparatus = appBall };
            var discGroup = new Discipline { Type = "Групова вправа (5 обручів)", Apparatus = appHoop };
            context.Disciplines.AddRange(discHoop, discBall, discGroup);

            var catSeniors = new Category { Type = "Сеньйорки", MinAge = 16, MaxAge = 99 };
            var catJuniors = new Category { Type = "Юніорки", MinAge = 13, MaxAge = 15 };
            context.Categories.AddRange(catSeniors, catJuniors);

            var comp1 = new Competition { Title = "Чемпіонат України 2026", City = "Київ", Country = "Україна", Level = CompetitionLevel.National, StartDate = DateTime.UtcNow.AddMonths(-3), EndDate = DateTime.UtcNow.AddMonths(-3).AddDays(5), Status = CompetitionStatus.Finished };
            var comp2 = new Competition { Title = "Кубок Львова 2026", City = "Львів", Country = "Україна", Level = CompetitionLevel.Local, StartDate = DateTime.UtcNow.AddMonths(-2), EndDate = DateTime.UtcNow.AddMonths(-2).AddDays(3), Status = CompetitionStatus.Finished };
            var comp3 = new Competition { Title = "Kyiv International RG Cup 2026", City = "Київ", Country = "Україна", Level = CompetitionLevel.International, StartDate = DateTime.UtcNow.AddMonths(-1), EndDate = DateTime.UtcNow.AddMonths(-1).AddDays(4), Status = CompetitionStatus.Finished };
            var comp4 = new Competition { Title = "Warsaw Spring Invitational 2026", City = "Варшава", Country = "Польща", Level = CompetitionLevel.International, StartDate = DateTime.UtcNow.AddMonths(2), EndDate = DateTime.UtcNow.AddMonths(2).AddDays(3), Status = CompetitionStatus.Planned };

            context.Competitions.AddRange(comp1, comp2, comp3, comp4);

            var jP1 = new Person { Name = "Наталія", Surname = "Степанова", Country = "Україна", Type = "Person" };
            var jP2 = new Person { Name = "Олександра", Surname = "Біла", Country = "Україна", Type = "Person" };
            var jP3 = new Person { Name = "Тетяна", Surname = "Чорна", Country = "Україна", Type = "Person" };
            context.Persons.AddRange(jP1, jP2, jP3);

            var judge1 = new Judge { Person = jP1, QualificationLevel = "Міжнародна" };
            var judge2 = new Judge { Person = jP2, QualificationLevel = "Міжнародна" };
            var judge3 = new Judge { Person = jP3, QualificationLevel = "Національна" };
            context.Judges.AddRange(judge1, judge2, judge3);

            var entries = new List<Entry>();
            foreach (var athlete in allAthletes)
            {
                foreach (var comp in new[] { comp1, comp2, comp3 })
                {
                    entries.Add(new Entry { Competition = comp, Participant = athlete, Discipline = discHoop, Category = catJuniors, ApplicationStatus = ApplicationStatus.Accepted, EntryStatus = EntryStatus.Finished, SubmittedAt = DateTime.UtcNow.AddMonths(-4) });
                }
            }
            var teamEntries = new List<Entry>
            {
                new Entry { Competition = comp1, Participant = teamKyiv, Discipline = discGroup, Category = catSeniors, ApplicationStatus = ApplicationStatus.Accepted, EntryStatus = EntryStatus.Finished, SubmittedAt = DateTime.UtcNow.AddMonths(-4) },
                new Entry { Competition = comp2, Participant = teamLviv, Discipline = discGroup, Category = catSeniors, ApplicationStatus = ApplicationStatus.Accepted, EntryStatus = EntryStatus.Finished, SubmittedAt = DateTime.UtcNow.AddMonths(-3) },
                new Entry { Competition = comp3, Participant = teamKyiv, Discipline = discGroup, Category = catSeniors, ApplicationStatus = ApplicationStatus.Accepted, EntryStatus = EntryStatus.Finished, SubmittedAt = DateTime.UtcNow.AddMonths(-2) }
            };

            context.Entries.AddRange(entries);
            context.Entries.AddRange(teamEntries);

            foreach (var comp in new[] { comp1, comp2, comp3 })
            {
                for (int i = 0; i < allAthletes.Count; i++)
                {
                    var entry = entries.FirstOrDefault(e => e.Competition == comp && e.Participant == allAthletes[i]);
                    if (entry == null) continue;

                    context.Scores.AddRange(
                        new Score { Entry = entry, Judge = judge1, Type = ScoreType.D, ScoreValue = 13.0f + i * 0.1f },
                        new Score { Entry = entry, Judge = judge2, Type = ScoreType.A, ScoreValue = 8.5f + i * 0.05f },
                        new Score { Entry = entry, Judge = judge3, Type = ScoreType.E, ScoreValue = 8.0f + i * 0.05f }
                    );

                    var place = i + 1;
                    var medal = place == 1 ? "Золото" : (place == 2 ? "Срібло" : (place == 3 ? "Бронза" : ""));
                    context.Results.Add(new Result { Entry = entry, Place = place, FinalScore = 29.5f - i * 0.2f, AwardedMedal = medal });
                }

                var tEntry = teamEntries.FirstOrDefault(e => e.Competition == comp);
                if (tEntry != null)
                {
                    context.Scores.AddRange(
                        new Score { Entry = tEntry, Judge = judge1, Type = ScoreType.D, ScoreValue = 15.0f },
                        new Score { Entry = tEntry, Judge = judge2, Type = ScoreType.A, ScoreValue = 9.0f },
                        new Score { Entry = tEntry, Judge = judge3, Type = ScoreType.E, ScoreValue = 9.0f }
                    );
                    context.Results.Add(new Result { Entry = tEntry, Place = 1, FinalScore = 33.0f, AwardedMedal = "Золото" });
                }
            }

            context.SaveChanges();
        }

        private static void EnsureCompetitionTypeSamples(CompetitionsTrackingDbContext context)
        {
            var sampleCompetitions = new List<Competition>
            {
                new Competition { Title = "Кубок Львова 2026", City = "Львів", Country = "Україна", Level = CompetitionLevel.Local, Status = CompetitionStatus.Finished },
                new Competition { Title = "Чемпіонат України 2026", City = "Київ", Country = "Україна", Level = CompetitionLevel.National, Status = CompetitionStatus.Finished },
                new Competition { Title = "Kyiv International RG Cup 2026", City = "Київ", Country = "Україна", Level = CompetitionLevel.International, Status = CompetitionStatus.Finished }
            };

            foreach (var sample in sampleCompetitions)
            {
                var existing = context.Competitions.FirstOrDefault(c => c.Title == sample.Title);
                if (existing != null)
                {
                    existing.Status = CompetitionStatus.Finished;
                    existing.Level = sample.Level;
                }
                else
                {
                    context.Competitions.Add(sample);
                }
            }
            context.SaveChanges();
        }

        private static void EnsureResultsForFinishedCompetitions(CompetitionsTrackingDbContext context)
        {
            var finishedComps = context.Competitions.Where(c => c.Status == CompetitionStatus.Finished).ToList();
            var athletes = context.Participants.OfType<Person>().Take(5).ToList();
            var teams = context.Participants.OfType<Team>().Take(2).ToList();
            var judges = context.Judges.Take(3).ToList();
            var disc = context.Disciplines.FirstOrDefault();
            var cat = context.Categories.FirstOrDefault();

            if (!athletes.Any() || judges.Count < 3 || disc == null || cat == null) return;

            foreach (var comp in finishedComps)
            {
                var existingEntries = context.Entries.Where(e => e.CompetitionId == comp.Id).ToList();
                if (existingEntries.Any())
                {
                    var entryIds = existingEntries.Select(e => e.Id).ToList();
                    var existingScores = context.Scores.Where(s => entryIds.Contains(s.EntryId)).ToList();
                    var existingResults = context.Results.Where(r => entryIds.Contains(r.EntryId)).ToList();
                    
                    context.Scores.RemoveRange(existingScores);
                    context.Results.RemoveRange(existingResults);
                    context.Entries.RemoveRange(existingEntries);
                    context.SaveChanges();
                }

                for (int i = 0; i < athletes.Count; i++)
                {
                    var athlete = athletes[i];
                    var entry = new Entry { CompetitionId = comp.Id, ParticipantId = athlete.Id, DisciplineId = disc.Id, CategoryId = cat.Id, ApplicationStatus = ApplicationStatus.Accepted, EntryStatus = EntryStatus.Finished, SubmittedAt = DateTime.UtcNow };
                    context.Entries.Add(entry);
                    context.SaveChanges();

                    context.Scores.AddRange(
                        new Score { EntryId = entry.Id, JudgeId = judges[0].Id, Type = ScoreType.D, ScoreValue = 13.0f },
                        new Score { EntryId = entry.Id, JudgeId = judges[1].Id, Type = ScoreType.A, ScoreValue = 8.5f },
                        new Score { EntryId = entry.Id, JudgeId = judges[2].Id, Type = ScoreType.E, ScoreValue = 8.0f }
                    );
                    context.Results.Add(new Result { EntryId = entry.Id, Place = i + 1, FinalScore = 29.5f, AwardedMedal = i == 0 ? "Золото" : (i == 1 ? "Срібло" : (i == 2 ? "Бронза" : "")) });
                }

                var groupDisc = context.Disciplines.FirstOrDefault(d => d.Type.Contains("Групова")) ?? disc;

                for (int i = 0; i < teams.Count; i++)
                {
                    var team = teams[i];
                    var tEntry = new Entry { CompetitionId = comp.Id, ParticipantId = team.Id, DisciplineId = groupDisc.Id, CategoryId = cat.Id, ApplicationStatus = ApplicationStatus.Accepted, EntryStatus = EntryStatus.Finished, SubmittedAt = DateTime.UtcNow };
                    context.Entries.Add(tEntry);
                    context.SaveChanges();
                    
                    var place = i + 1;
                    context.Results.Add(new Result { EntryId = tEntry.Id, Place = place, FinalScore = 32.0f - i * 0.5f, AwardedMedal = place == 1 ? "Золото" : "Срібло" });
                }
            }
            context.SaveChanges();
        }

        private static void EnsureAtLeastOneFinishedPerLevel(CompetitionsTrackingDbContext context)
        {
            foreach (CompetitionLevel level in Enum.GetValues(typeof(CompetitionLevel)))
            {
                if (!context.Competitions.Any(c => c.Level == level && c.Status == CompetitionStatus.Finished))
                {
                    var comp = context.Competitions.FirstOrDefault(c => c.Level == level);
                    if (comp != null) comp.Status = CompetitionStatus.Finished;
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
