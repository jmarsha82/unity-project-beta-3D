using NUnit.Framework;
using UnityEngine;

namespace StealthGame.Tests
{
    public class HauntedHouseEnhancementsTests
    {
        [Test]
        public void CalculateThreat01_ReturnsFullThreatAtZeroDistance()
        {
            Assert.That(global::HauntedHouseEnhancements.CalculateThreat01(0f, 8f), Is.EqualTo(1f));
        }

        [Test]
        public void CalculateThreat01_ReturnsNoThreatOutsideRange()
        {
            Assert.That(global::HauntedHouseEnhancements.CalculateThreat01(12f, 8f), Is.EqualTo(0f));
        }

        [Test]
        public void CalculateStamina_WhenSprinting_DrainsAndClamps()
        {
            float stamina = global::HauntedHouseEnhancements.CalculateStamina(0.1f, true, 1f, 0.35f, 0.2f);

            Assert.That(stamina, Is.EqualTo(0f));
        }

        [Test]
        public void CalculateStamina_WhenResting_RecoversAndClamps()
        {
            float stamina = global::HauntedHouseEnhancements.CalculateStamina(0.9f, false, 1f, 0.35f, 0.2f);

            Assert.That(stamina, Is.EqualTo(1f));
        }

        [Test]
        public void GetWaypointRouteName_ReturnsRoutePrefix()
        {
            Assert.That(global::HauntedHouseEnhancements.GetWaypointRouteName("Waypoint_4_Start"), Is.EqualTo("Waypoint_4"));
        }

        [Test]
        public void Collect_OnlyCountsCollectibleOnce()
        {
            GameObject controllerObject = new GameObject("Enhancements");
            GameObject collectibleObject = new GameObject("Orb");
            try
            {
                global::HauntedHouseEnhancements controller = controllerObject.AddComponent<global::HauntedHouseEnhancements>();
                controller.collectibleGoal = 5;
                global::HauntedCollectible collectible = collectibleObject.AddComponent<global::HauntedCollectible>();
                collectible.Initialize(controller, null);

                controller.Collect(collectible);
                controller.Collect(collectible);

                Assert.That(controller.CollectedCount, Is.EqualTo(1));
                Assert.That(collectible.IsCollected, Is.True);
            }
            finally
            {
                Object.DestroyImmediate(controllerObject);
                Object.DestroyImmediate(collectibleObject);
            }
        }
    }
}
