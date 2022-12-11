/*
Copyright 2022 Boonebytes

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using DiscordBot.Domain.Exceptions;
using DiscordBot.Domain.Seedwork;

namespace DiscordBot.Domain.Entities.Admin;

public class JobStatus : Enumeration
{
    public static JobStatus Unspecified = new(0, nameof(Unspecified), "Unspecified");
    public static JobStatus Scheduled = new(1, nameof(Scheduled), "Scheduled");
    public static JobStatus Completed = new(2, nameof(Completed), "Completed");
    public static JobStatus Cancelled = new(3, nameof(Cancelled), "Cancelled");
    public static JobStatus Failed = new(4, nameof(Failed), "Failed");

    public virtual string Label { get; private set; }

    public JobStatus(int id, string name, string label)
        : base(id, name)
    {
        this.Label = label;
    }

    public static IEnumerable<JobStatus> List() =>
        new[] { Unspecified, Scheduled, Completed, Cancelled, Failed };

    public static JobStatus FromName(string name)
    {
        var res = List()
            .SingleOrDefault(s => String.Equals(s.Name, name, StringComparison.CurrentCultureIgnoreCase));

        if (res == null)
        {
            throw new BotDomainException($"Possible values for JobStatus: {String.Join(",", List().Select(s => s.Name))}");
        }

        return res;
    }

    public static JobStatus From(int id)
    {
        var res = List().SingleOrDefault(s => s.Id == id);

        if (res == null)
        {
            throw new BotDomainException($"Possible values for JobStatus: {String.Join(",", List().Select(s => s.Name))}");
        }

        return res;
    }
}