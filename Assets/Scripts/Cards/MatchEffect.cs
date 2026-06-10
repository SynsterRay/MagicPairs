using UnityEngine;
using MagicPairs.Core;

namespace MagicPairs.Cards
{
    public class MatchEffect : MonoBehaviour
    {
        private static Material _particleMat;

        private void OnEnable() => GameEvents.OnPairMatched += PlayEffect;
        private void OnDisable() => GameEvents.OnPairMatched -= PlayEffect;

        private void PlayEffect(int playerIndex, int colorIndex)
        {
            var grid = GetComponent<CardGrid>();
            if (grid == null) return;

            foreach (var card in grid.Cards)
            {
                if (card != null && card.State == CardState.Matched && card.Data.colorIndex == colorIndex)
                {
                    SpawnParticles(card.transform.position, card.Data.faceColor);
                }
            }
        }

        private void SpawnParticles(Vector3 position, Color color)
        {
            var go = new GameObject("MatchFX");
            go.transform.position = position + Vector3.back * 0.5f;

            var ps = go.AddComponent<ParticleSystem>();
            ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

            var main = ps.main;
            main.duration = 0.6f;
            main.startLifetime = 0.8f;
            main.startSpeed = 3f;
            main.startSize = 0.15f;
            main.startColor = new ParticleSystem.MinMaxGradient(color, Color.white);
            main.gravityModifier = 1.5f;
            main.maxParticles = 30;
            main.loop = false;
            main.playOnAwake = false;
            main.simulationSpace = ParticleSystemSimulationSpace.World;

            var emission = ps.emission;
            emission.rateOverTime = 0f;
            emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 20) });

            var shape = ps.shape;
            shape.shapeType = ParticleSystemShapeType.Circle;
            shape.radius = 0.3f;

            var sizeOverLifetime = ps.sizeOverLifetime;
            sizeOverLifetime.enabled = true;
            sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, AnimationCurve.Linear(0f, 1f, 1f, 0f));

            var renderer = go.GetComponent<ParticleSystemRenderer>();
            if (_particleMat == null)
                _particleMat = new Material(Shader.Find("Particles/Standard Unlit")
                    ?? Shader.Find("Universal Render Pipeline/Particles/Unlit")
                    ?? Shader.Find("Sprites/Default"));
            renderer.material = _particleMat;

            ps.Play();
            Destroy(go, 1.5f);
        }
    }
}
