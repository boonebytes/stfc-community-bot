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

public class StarbaseInventory
{
    public DateTime EffectiveDate { get; protected set; }
    public decimal Reserves { get; protected set; }
    public decimal ReservesDelta { get; protected set; }
    public decimal CollisionalPlasma { get; protected set; }
    public decimal CollisionalPlasmaDelta { get; protected set; }
    public decimal MagneticPlasma { get; protected set; }
    public decimal MagneticPlasmaDelta { get; protected set; }
    public decimal Superconductors { get; protected set; }
    public decimal SuperconductorsDelta { get; protected set; }
}