import { provideHttpClient } from '@angular/common/http';
import { provideHttpClientTesting } from '@angular/common/http/testing';
import { TestBed } from '@angular/core/testing';
import { provideRouter } from '@angular/router';
import { of, throwError } from 'rxjs';
import { DamageApiService } from '../../core/services/damage-api.service';
import { LanguageService } from '../../core/services/language.service';
import { Rotation, RotationResult } from '../../core/models/damage.models';
import { RotationComponent } from './rotation.component';

describe('RotationComponent', () => {
  let damageApi: DamageApiService;

  const sampleResult: RotationResult = {
    steps: [
      { name: 'Hit 1', damage: { nonCritical: 500, critical: 1000, average: 750 } },
      { name: 'Reaction 1', damage: { nonCritical: 2000, critical: 2000, average: 2000 } },
    ],
    totalNonCritical: 2500,
    totalCritical: 3000,
    totalAverage: 2750,
    dps: 275,
  };

  beforeEach(() => {
    TestBed.configureTestingModule({
      imports: [RotationComponent],
      providers: [provideRouter([]), provideHttpClient(), provideHttpClientTesting()],
    });
    damageApi = TestBed.inject(DamageApiService);
  });

  it('starts with no steps and no result', () => {
    const fixture = TestBed.createComponent(RotationComponent);
    fixture.detectChanges();

    expect(fixture.componentInstance.steps()).toEqual([]);
    expect(fixture.componentInstance.result()).toBeNull();
  });

  it('addHitStep and addTransformativeStep append steps with unique ids and sensible defaults', () => {
    const fixture = TestBed.createComponent(RotationComponent);
    const component = fixture.componentInstance;

    component.addHitStep();
    component.addTransformativeStep();

    const steps = component.steps();
    expect(steps.length).toBe(2);
    expect(steps[0].kind).toBe('hit');
    expect(steps[1].kind).toBe('transformative');
    expect(steps[0].id).not.toBe(steps[1].id);
    expect(component.asHit(steps[0]).critRatePercent).toBe(5);
    expect(component.asTransformative(steps[1]).reaction).toBe('Overloaded');
  });

  it('removeStep removes only the targeted step', () => {
    const fixture = TestBed.createComponent(RotationComponent);
    const component = fixture.componentInstance;
    component.addHitStep();
    component.addHitStep();
    const [first, second] = component.steps();

    component.removeStep(first.id);

    expect(component.steps()).toEqual([second]);
  });

  it('moveStep swaps adjacent steps and is a no-op at the boundaries', () => {
    const fixture = TestBed.createComponent(RotationComponent);
    const component = fixture.componentInstance;
    component.addHitStep();
    component.addTransformativeStep();
    const [first, second] = component.steps();

    component.moveStep(second.id, -1);
    expect(component.steps().map((s) => s.id)).toEqual([second.id, first.id]);

    component.moveStep(second.id, -1); // already first: no-op
    expect(component.steps().map((s) => s.id)).toEqual([second.id, first.id]);

    component.moveStep(first.id, 1); // already last: no-op
    expect(component.steps().map((s) => s.id)).toEqual([second.id, first.id]);
  });

  it('calculate does nothing when there are no steps', () => {
    spyOn(damageApi, 'calculateRotation');
    const fixture = TestBed.createComponent(RotationComponent);

    fixture.componentInstance.calculate();

    expect(damageApi.calculateRotation).not.toHaveBeenCalled();
  });

  it('calculate builds a Rotation payload with percent values converted to fractions and calls the API', () => {
    const spy = spyOn(damageApi, 'calculateRotation').and.returnValue(of(sampleResult));
    const fixture = TestBed.createComponent(RotationComponent);
    const component = fixture.componentInstance;
    component.rotationName.set('Test rotation');
    component.durationSeconds.set(8);
    component.context.update((c) => ({ ...c, characterLevel: 80, elementalMastery: 300, enemyResistancePercent: 20 }));
    component.addHitStep();
    component.asHit(component.steps()[0]).critRatePercent = 70;
    component.asHit(component.steps()[0]).critDamagePercent = 140;
    component.asHit(component.steps()[0]).reactionKey = 'vaporize-pyro';

    component.calculate();

    expect(spy).toHaveBeenCalledTimes(1);
    const payload: Rotation = spy.calls.mostRecent().args[0];
    expect(payload.name).toBe('Test rotation');
    expect(payload.durationSeconds).toBe(8);
    expect(payload.steps.length).toBe(1);
    const hit = payload.steps[0].hit!;
    expect(hit.characterLevel).toBe(80);
    expect(hit.elementalMastery).toBe(300);
    expect(hit.critRate).toBeCloseTo(0.7);
    expect(hit.critDamage).toBeCloseTo(1.4);
    expect(hit.amplifying).toBe('Vaporize');
    expect(hit.triggerElement).toBe('Pyro');
    expect(hit.enemy.resistance).toBeCloseTo(0.2);

    expect(component.result()).toEqual(sampleResult);
    expect(component.loading()).toBeFalse();
  });

  it('calculate maps a transformative step without a hit payload', () => {
    const spy = spyOn(damageApi, 'calculateRotation').and.returnValue(of(sampleResult));
    const fixture = TestBed.createComponent(RotationComponent);
    const component = fixture.componentInstance;
    component.addTransformativeStep();
    component.asTransformative(component.steps()[0]).reaction = 'LunarCharged';
    component.asTransformative(component.steps()[0]).reactionBonusPercent = 10;

    component.calculate();

    const payload: Rotation = spy.calls.mostRecent().args[0];
    expect(payload.steps[0].hit).toBeUndefined();
    const transformative = payload.steps[0].transformative!;
    expect(transformative.reaction).toBe('LunarCharged');
    expect(transformative.reactionBonus).toBeCloseTo(0.1);
  });

  it('calculate sets a bilingual error message and stops loading on failure', () => {
    spyOn(damageApi, 'calculateRotation').and.returnValue(throwError(() => new Error('boom')));
    const fixture = TestBed.createComponent(RotationComponent);
    const component = fixture.componentInstance;
    component.addHitStep();

    component.calculate();

    expect(component.error()).toContain('Failed to calculate');
    expect(component.loading()).toBeFalse();
    expect(component.result()).toBeNull();
  });

  it('hitReactionLabel reflects the active language', () => {
    const fixture = TestBed.createComponent(RotationComponent);
    const component = fixture.componentInstance;
    component.addHitStep();
    const step = component.asHit(component.steps()[0]);
    step.reactionKey = 'aggravate';

    expect(component.hitReactionLabel(step)).toBe('Aggravate');
    TestBed.inject(LanguageService).set('ru');
    expect(component.hitReactionLabel(step)).toBe('Обострение');
  });
});
