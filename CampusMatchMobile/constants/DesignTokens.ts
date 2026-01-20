/**
 * Spacing System
 * Use these constants for all padding, margin, and gap values
 */
export const Spacing = {
    /** 4px - Minimal spacing */
    xs: 4,
    /** 8px - Small spacing */
    sm: 8,
    /** 12px - Medium spacing */
    md: 12,
    /** 16px - Standard spacing */
    lg: 16,
    /** 20px - Large spacing */
    xl: 20,
    /** 24px - Extra large spacing */
    xxl: 24,
    /** 32px - Section spacing */
    xxxl: 32,
    /** 48px - Major section spacing */
    huge: 48,
} as const;

/**
 * Border Radius System
 * Use these for consistent rounded corners
 */
export const BorderRadius = {
    /** 8px - Small radius */
    sm: 8,
    /** 12px - Medium radius (inputs, small cards) */
    md: 12,
    /** 16px - Large radius (buttons, cards) */
    lg: 16,
    /** 20px - Extra large radius */
    xl: 20,
    /** 24px - XXL radius (swipe cards) */
    xxl: 24,
    /** 999px - Fully rounded (pills, circles) */
    round: 999,
} as const;

/**
 * Typography System
 * Use these for consistent text styling
 */
export const Typography = {
    h1: {
        fontSize: 32,
        fontWeight: '700' as const,
        lineHeight: 40,
    },
    h2: {
        fontSize: 24,
        fontWeight: '600' as const,
        lineHeight: 32,
    },
    h3: {
        fontSize: 20,
        fontWeight: '600' as const,
        lineHeight: 28,
    },
    h4: {
        fontSize: 18,
        fontWeight: '600' as const,
        lineHeight: 24,
    },
    body: {
        fontSize: 16,
        fontWeight: '400' as const,
        lineHeight: 24,
    },
    bodyLarge: {
        fontSize: 17,
        fontWeight: '400' as const,
        lineHeight: 26,
    },
    bodySmall: {
        fontSize: 14,
        fontWeight: '400' as const,
        lineHeight: 20,
    },
    caption: {
        fontSize: 12,
        fontWeight: '400' as const,
        lineHeight: 16,
    },
    captionSmall: {
        fontSize: 11,
        fontWeight: '400' as const,
        lineHeight: 14,
    },
    button: {
        fontSize: 16,
        fontWeight: '600' as const,
        lineHeight: 20,
    },
    buttonLarge: {
        fontSize: 18,
        fontWeight: '600' as const,
        lineHeight: 24,
    },
} as const;

/**
 * Shadow System
 * Use these for consistent elevation
 */
export const Shadows = {
    sm: {
        shadowColor: '#000',
        shadowOffset: { width: 0, height: 2 },
        shadowOpacity: 0.1,
        shadowRadius: 4,
        elevation: 2,
    },
    md: {
        shadowColor: '#000',
        shadowOffset: { width: 0, height: 4 },
        shadowOpacity: 0.15,
        shadowRadius: 8,
        elevation: 4,
    },
    lg: {
        shadowColor: '#000',
        shadowOffset: { width: 0, height: 8 },
        shadowOpacity: 0.2,
        shadowRadius: 16,
        elevation: 8,
    },
    xl: {
        shadowColor: '#000',
        shadowOffset: { width: 0, height: 12 },
        shadowOpacity: 0.3,
        shadowRadius: 24,
        elevation: 12,
    },
} as const;

/**
 * Animation Durations (ms)
 * Use for consistent timing across transitions
 */
export const Animation = {
    instant: 100,
    fast: 200,
    normal: 300,
    slow: 500,
} as const;

/**
 * Sizing System
 * Use for buttons, inputs, avatars, and touch targets
 */
export const Sizing = {
    /** Minimum touch target for accessibility (44x44) */
    touchTarget: 44,

    icon: {
        sm: 18,
        md: 22,
        lg: 24,
        xl: 28,
    },

    avatar: {
        sm: 40,
        md: 56,
        lg: 80,
        xl: 112,
    },

    button: {
        sm: 36,
        md: 44,
        lg: 52,
    },

    input: {
        sm: 40,
        md: 48,
        lg: 56,
    },
} as const;

/**
 * Accessibility
 * Constants and defaults for a11y compliance
 */
export const Accessibility = {
    /** Minimum touch target size (WCAG 2.1 Level AAA) */
    minTouchTarget: 44,
    /** Minimum contrast ratio for normal text */
    minContrastRatio: 4.5,
    /** Minimum contrast ratio for large text */
    minLargeTextContrastRatio: 3,
} as const;

