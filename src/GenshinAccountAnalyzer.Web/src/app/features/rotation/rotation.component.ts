import { DecimalPipe } from '@angular/common';
import { Component, computed, inject, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ApexOptions, ChartComponent } from 'ng-apexcharts';
import { DamageApiService } from '../../core/services/damage-api.service';
import { LanguageService } from '../../core/services/language.service';
import {
  DamageInput,
  EnemyProfile,
  Rotation,
  RotationResult,
  RotationStep,
  TransformativeHit,
} from '../../core/models/damage.models';
import { ScalingType, TransformativeReaction } from '../../core/models/enums';
import { HIT_REACTION_OPTIONS, HitReactionOption, TRANSFORMATIVE_REACTION_LABELS } from '../../core/reaction-labels';

interface HitStepDraft {
  kind: 'hit';
  id: number;
  name: string;
  scaling: ScalingType;
  talentMultiplierPercent: number;
  scalingStatValue: number;
  flatDamageBonus: number;
  damageBonusPercent: number;
  critRatePercent: number;
  critDamagePercent: number;
  reactionKey: string;
  reactionBonusPercent: number;
}

interface TransformativeStepDraft {
  kind: 'transformative';
  id: number;
  name: string;
  reaction: TransformativeReaction;
  reactionBonusPercent: number;
}

type StepDraft = HitStepDraft | TransformativeStepDraft;

interface RotationContext {
  characterLevel: number;
  elementalMastery: number;
  enemyLevel: number;
  enemyResistancePercent: number;
  enemyResistanceReductionPercent: number;
  enemyDefenseReductionPercent: number;
  enemyDefenseIgnorePercent: number;
}

const DEFAULT_CONTEXT: RotationContext = {
  characterLevel: 90,
  elementalMastery: 200,
  enemyLevel: 90,
  enemyResistancePercent: 10,
  enemyResistanceReductionPercent: 0,
  enemyDefenseReductionPercent: 0,
  enemyDefenseIgnorePercent: 0,
};

/**
 * A DPS "rotation" builder: chain hits and reaction procs against a shared character/enemy context and
 * see per-step damage plus the rotation's floor (no crits), maximum (all crits), expected total and DPS.
 */
@Component({
  selector: 'app-rotation',
  imports: [FormsModule, ChartComponent, DecimalPipe],
  templateUrl: './rotation.component.html',
  styleUrl: './rotation.component.scss',
})
export class RotationComponent {
  private readonly damageApi = inject(DamageApiService);
  protected readonly language = inject(LanguageService);

  protected readonly hitReactionOptions: readonly HitReactionOption[] = HIT_REACTION_OPTIONS;
  protected readonly transformativeReactions = Object.keys(
    TRANSFORMATIVE_REACTION_LABELS,
  ) as TransformativeReaction[];
  protected readonly transformativeLabels = TRANSFORMATIVE_REACTION_LABELS;

  readonly rotationName = signal('My rotation');
  readonly durationSeconds = signal(0);
  readonly context = signal<RotationContext>({ ...DEFAULT_CONTEXT });
  readonly steps = signal<StepDraft[]>([]);

  readonly result = signal<RotationResult | null>(null);
  readonly loading = signal(false);
  readonly error = signal<string | null>(null);

  private nextId = 1;

  protected readonly chartOptions = computed<ApexOptions>(() => {
    const r = this.result();
    const names = r ? r.steps.map((s) => s.name) : [];
    const data = r ? r.steps.map((s) => Math.round(s.damage.average)) : [];

    return {
      chart: {
        type: 'bar',
        height: Math.max(220, names.length * 34),
        background: 'transparent',
        toolbar: { show: false },
        foreColor: '#9aa0c8',
      },
      series: [{ name: 'Average damage', data }],
      xaxis: { categories: names },
      plotOptions: { bar: { horizontal: true, borderRadius: 3 } },
      colors: ['#7c8cff'],
      dataLabels: { enabled: true },
      legend: { show: false },
    };
  });

  addHitStep(): void {
    const draft: HitStepDraft = {
      kind: 'hit',
      id: this.nextId++,
      name: `${this.language.language() === 'ru' ? 'Хит' : 'Hit'} ${this.steps().length + 1}`,
      scaling: 'Atk',
      talentMultiplierPercent: 100,
      scalingStatValue: 2000,
      flatDamageBonus: 0,
      damageBonusPercent: 0,
      critRatePercent: 5,
      critDamagePercent: 50,
      reactionKey: 'none',
      reactionBonusPercent: 0,
    };
    this.steps.update((list) => [...list, draft]);
  }

  addTransformativeStep(): void {
    const draft: TransformativeStepDraft = {
      kind: 'transformative',
      id: this.nextId++,
      name: `${this.language.language() === 'ru' ? 'Реакция' : 'Reaction'} ${this.steps().length + 1}`,
      reaction: 'Overloaded',
      reactionBonusPercent: 0,
    };
    this.steps.update((list) => [...list, draft]);
  }

  updateContext<K extends keyof RotationContext>(key: K, value: RotationContext[K]): void {
    this.context.update((c) => ({ ...c, [key]: value }));
  }

  removeStep(id: number): void {
    this.steps.update((list) => list.filter((s) => s.id !== id));
  }

  moveStep(id: number, direction: -1 | 1): void {
    this.steps.update((list) => {
      const index = list.findIndex((s) => s.id === id);
      const target = index + direction;
      if (index < 0 || target < 0 || target >= list.length) {
        return list;
      }
      const next = [...list];
      [next[index], next[target]] = [next[target], next[index]];
      return next;
    });
  }

  asHit(step: StepDraft): HitStepDraft {
    return step as HitStepDraft;
  }

  asTransformative(step: StepDraft): TransformativeStepDraft {
    return step as TransformativeStepDraft;
  }

  hitReactionLabel(step: HitStepDraft): string {
    const option = this.hitReactionOptions.find((o) => o.key === step.reactionKey) ?? this.hitReactionOptions[0];
    return this.language.language() === 'ru' ? option.labelRu : option.labelEn;
  }

  calculate(): void {
    if (this.steps().length === 0) {
      return;
    }

    this.loading.set(true);
    this.error.set(null);

    const rotation: Rotation = {
      name: this.rotationName(),
      durationSeconds: this.durationSeconds(),
      steps: this.steps().map((draft) => this.buildStep(draft)),
    };

    this.damageApi.calculateRotation(rotation).subscribe({
      next: (result) => {
        this.result.set(result);
        this.loading.set(false);
      },
      error: () => {
        this.error.set(
          this.language.language() === 'ru'
            ? 'Не удалось рассчитать ротацию. Проверьте введённые значения.'
            : 'Failed to calculate the rotation. Check the values you entered.',
        );
        this.loading.set(false);
      },
    });
  }

  private buildEnemy(): EnemyProfile {
    const c = this.context();
    return {
      level: c.enemyLevel,
      resistance: c.enemyResistancePercent / 100,
      resistanceReduction: c.enemyResistanceReductionPercent / 100,
      defenseReduction: c.enemyDefenseReductionPercent / 100,
      defenseIgnore: c.enemyDefenseIgnorePercent / 100,
    };
  }

  private buildStep(draft: StepDraft): RotationStep {
    if (draft.kind === 'hit') {
      const reaction =
        this.hitReactionOptions.find((o) => o.key === draft.reactionKey) ?? this.hitReactionOptions[0];

      const hit: DamageInput = {
        characterLevel: this.context().characterLevel,
        talentMultiplier: draft.talentMultiplierPercent / 100,
        scaling: draft.scaling,
        scalingStatValue: draft.scalingStatValue,
        flatDamageBonus: draft.flatDamageBonus,
        damageBonus: draft.damageBonusPercent / 100,
        critRate: draft.critRatePercent / 100,
        critDamage: draft.critDamagePercent / 100,
        elementalMastery: this.context().elementalMastery,
        amplifying: reaction.amplifying,
        triggerElement: reaction.triggerElement,
        additive: reaction.additive,
        reactionBonus: draft.reactionBonusPercent / 100,
        enemy: this.buildEnemy(),
      };
      return { name: draft.name, hit };
    }

    const transformative: TransformativeHit = {
      reaction: draft.reaction,
      characterLevel: this.context().characterLevel,
      elementalMastery: this.context().elementalMastery,
      reactionBonus: draft.reactionBonusPercent / 100,
      enemy: this.buildEnemy(),
    };
    return { name: draft.name, transformative };
  }
}
