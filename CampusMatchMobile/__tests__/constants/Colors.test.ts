import { Colors } from '../../constants/Colors';
import { Spacing, BorderRadius, Typography, Shadows } from '../../constants/DesignTokens';

describe('Colors', () => {
    it('should have primary gradient colors', () => {
        expect(Colors.primary.gradient).toBeDefined();
        expect(Colors.primary.gradient.length).toBeGreaterThan(0);
    });

    it('should have dark theme colors', () => {
        expect(Colors.dark.background).toBeDefined();
        expect(Colors.dark.text).toBe('#FFFFFF');
        expect(Colors.dark.surface).toBeDefined();
    });

    it('should have action colors for swipe', () => {
        expect(Colors.like).toBeDefined();
        expect(Colors.dislike).toBeDefined();
        expect(Colors.superLike).toBeDefined();
    });

    it('should have gradient arrays for LinearGradient', () => {
        expect(Colors.gradients.dark).toHaveLength(3);
        expect(Colors.gradients.primary).toHaveLength(3);
    });
});

describe('DesignTokens - Spacing', () => {
    it('should have spacing values following 4px grid', () => {
        expect(Spacing.xs).toBe(4);
        expect(Spacing.sm).toBe(8);
        expect(Spacing.md).toBe(12);
        expect(Spacing.lg).toBe(16);
    });

    it('should have larger spacing values', () => {
        expect(Spacing.xl).toBe(20);
        expect(Spacing.xxl).toBe(24);
        expect(Spacing.xxxl).toBe(32);
        expect(Spacing.huge).toBe(48);
    });
});

describe('DesignTokens - BorderRadius', () => {
    it('should have border radius values', () => {
        expect(BorderRadius.sm).toBe(8);
        expect(BorderRadius.md).toBe(12);
        expect(BorderRadius.lg).toBe(16);
        expect(BorderRadius.round).toBe(999);
    });
});

describe('DesignTokens - Typography', () => {
    it('should have heading styles', () => {
        expect(Typography.h1.fontSize).toBe(32);
        expect(Typography.h2.fontSize).toBe(24);
        expect(Typography.h3.fontSize).toBe(20);
    });

    it('should have body styles', () => {
        expect(Typography.body.fontSize).toBe(16);
        expect(Typography.bodySmall.fontSize).toBe(14);
        expect(Typography.caption.fontSize).toBe(12);
    });
});

describe('DesignTokens - Shadows', () => {
    it('should have shadow definitions with elevation', () => {
        expect(Shadows.sm.elevation).toBe(2);
        expect(Shadows.md.elevation).toBe(4);
        expect(Shadows.lg.elevation).toBe(8);
        expect(Shadows.xl.elevation).toBe(12);
    });
});
