using System.Collections.Generic;
using System.Linq;
using Penchant.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Penchant.Editor
{
    public class PenchantVisualizer : EditorWindow
    {
        SeededRandom seededRandom;
        
        const int DEFAULT_SAMPLES = 10;
        string currentSeed;
        int currentSamples;
        
        VisualElement root;
        TextField seed;
        SliderInt samples;
        VisualElement samplesGraph;
        
        List<VisualElement> sampleBars = new();
        
        [MenuItem("Window/Penchant/Visualizer"), MenuItem("Penchant/Visualizer")]
        public static void Open()
        {
            GetWindow<PenchantVisualizer>("Penchant Visualizer");
        }
        
        void CreateGUI()
        {
            seededRandom = new SeededRandom();
            
            PenchantEditorUtilities.AddStyleSheet(rootVisualElement.styleSheets, "PenchantVisualizerStyleSheet");
            AddElements();

            seed.value = currentSeed;
            samples.value = currentSamples;
        }

        void AddElements()
        {
            root = new VisualElement();
            root.AddToClassList("root");
            root.AddToClassList("flex_column");
            rootVisualElement.Add(root);
            
            /* Seed */
            
            seed = new TextField("Seed:");
            root.Add(seed);
            
            seed.RegisterValueChangedCallback(evt =>
            {
                currentSeed = evt.newValue;
                seededRandom.Seed = evt.newValue;
                Reset();
            });
            
            /* Samples */

            VisualElement samplesRow = new VisualElement();
            samplesRow.AddToClassList("flex_row");
            root.Add(samplesRow);
            
            samples = new SliderInt($"Samples ({DEFAULT_SAMPLES}):", DEFAULT_SAMPLES, DEFAULT_SAMPLES * 10);
            samplesRow.Add(samples);
            var samplesLabel = PenchantEditorUtilities.FindFirstElement<Label>(samples);
            
            samples.RegisterValueChangedCallback(evt =>
            {
                currentSamples = evt.newValue;
                samplesLabel.text = $"Samples ({currentSamples}):";
                Reset();
            });

            Button decreaseSamples = new Button();
            decreaseSamples.text = "<";
            samplesRow.Add(decreaseSamples);

            decreaseSamples.clicked += () => { samples.value = Mathf.Clamp(samples.value - 1, DEFAULT_SAMPLES, DEFAULT_SAMPLES * 10); };

            Button increaseSamples = new Button();
            increaseSamples.text = ">";
            samplesRow.Add(increaseSamples);

            increaseSamples.clicked += () => { samples.value = Mathf.Clamp(samples.value + 1, DEFAULT_SAMPLES, DEFAULT_SAMPLES * 10); };
            
            /* Graph */
            
            samplesGraph = new VisualElement();
            samplesGraph.AddToClassList("samples_graph");
            samplesGraph.AddToClassList("flex_row");
            root.Add(samplesGraph);

            SetSampleBars(DEFAULT_SAMPLES, samplesGraph);
        }

        void Reset()
        {
            seededRandom.Reset();
            SetSampleBars(currentSamples, samplesGraph);
        }

        void SetSampleBars(int count, VisualElement parent)
        {
            foreach (var i in sampleBars)
                PenchantEditorUtilities.RemoveElement(i);
                
            sampleBars.Clear();
                
            for (int j = 0; j < count; j++)
            {
                var rand = seededRandom.RandomValue;
                
                Label bar = new Label();
                bar.AddToClassList("samples_bar");
                bar.AddToClassList("flex_column");
                parent.Add(bar);
                
                Label barModification = new Label();
                barModification.style.height = new StyleLength(new Length(500f * rand.Modification));
                barModification.AddToClassList("samples_bar_modification");
                bar.Add(barModification);
                
                Label barBase = new Label();
                barBase.style.height = new StyleLength(new Length(500f * rand.Base));
                barBase.AddToClassList("samples_bar_base");
                bar.Add(barBase);

                int maxSamples = 35;
                
                int decimalPlaces = (int)(-0.2f * count + 8f);
                float scaleFactor = Mathf.Pow(10, decimalPlaces);
                string roundedValue = (Mathf.RoundToInt(rand * scaleFactor) / scaleFactor).ToString();

                if (count < maxSamples)
                {
                    if (!roundedValue.Contains('.')) roundedValue += '.';
                    roundedValue = roundedValue.PadRight(decimalPlaces + 2, '0');   
                }
                
                Label barValue = new Label(count < maxSamples
                    ? roundedValue
                    : "");
                bar.Add(barValue);
                    
                sampleBars.Add(bar);
            }
        }
    }
}