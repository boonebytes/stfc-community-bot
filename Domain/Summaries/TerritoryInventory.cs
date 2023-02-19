/*
Copyright 2023 Boonebytes

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

namespace DiscordBot.Domain.Summaries;

public class TerritoryInventory
{
    public DateTime EffectiveDate { get; protected set; }
    public decimal Isogen1 { get; protected set; }
    public decimal Isogen1Delta { get; protected set; }
    public decimal Isogen2 { get; protected set; }
    public decimal Isogen2Delta { get; protected set; }
    public decimal Isogen3 { get; protected set; }
    public decimal Isogen3Delta { get; protected set; }
    public decimal Cores { get; protected set; }
    public decimal CoresDelta { get; protected set; }
    public decimal Diodes { get; protected set; }
    public decimal DiodesDelta { get; protected set; }
    public decimal Emitters { get; protected set; }
    public decimal EmittersDelta { get; protected set; }
    public decimal Reactors { get; protected set; }
    public decimal ReactorsDelta { get; protected set; }
}