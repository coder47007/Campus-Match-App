// Premium color palette for CampusMatch
// Modern dating app aesthetic with Deep Purple / Magenta theme

export const Colors = {
  // Primary gradient colors (Magenta / Deep Purple theme)
  primary: {
    gradient: ['#FF4081', '#D500F9', '#7C3AED'] as const,
    main: '#FF4081', // Neon Pink
    light: '#FF80AB',
    dark: '#C51162',
  },

  // Secondary accent colors
  secondary: {
    gradient: ['#E1BEE7', '#4A148C'] as const,
    main: '#E1BEE7', // Pale Purple
    light: '#F3E5F5',
    dark: '#4A148C', // Dark Purple
  },

  // Action colors
  like: '#00E676',           // Neon Green
  superLike: '#2979FF',      // Azure Blue
  dislike: '#FF1744',        // Neon Red
  rewind: '#FFCA28',         // Amber
  boost: '#E040FB',          // Purple Boost

  // Match celebration
  match: {
    gradient: ['#FF4081', '#7C3AED', '#660c6c'] as const,
  },

  // Enforce Deep Purple theme for Light mode as well
  light: {
    background: '#660c6c',     // Deep Magenta (Requested)
    surface: '#1a1a2e',        // Dark Blue/Purple (from Login Page)
    surfaceElevated: '#2A0E35', // Darker Magenta Card
    card: '#2A0E35',           // Darker Magenta Card
    text: '#FFFFFF',
    textSecondary: '#E1BEE7',  // Pale Purple
    textMuted: '#9C27B0',      // Purple Muted
    border: '#4A148C',         // Dark Purple
    borderStrong: 'rgba(225, 190, 231, 0.3)',
    icon: '#E1BEE7',

    // Overlay system
    overlay: {
      light: 'rgba(26, 26, 46, 0.3)',
      medium: 'rgba(26, 26, 46, 0.5)',
      heavy: 'rgba(26, 26, 46, 0.85)',
    },
  },

  // Dark theme - Deep Purple
  dark: {
    background: '#660c6c',     // Deep Magenta (Requested)
    surface: '#1a1a2e',        // Dark Blue/Purple (from Login Page)
    surfaceElevated: '#2A0E35', // Darker Magenta Card
    card: '#2A0E35',           // Darker Magenta Card
    text: '#FFFFFF',
    textSecondary: '#E1BEE7',
    textMuted: '#9C27B0',
    textDisabled: 'rgba(255, 255, 255, 0.3)',
    border: '#4A148C',
    borderStrong: 'rgba(225, 190, 231, 0.3)',
    icon: '#E1BEE7',

    // Overlay system
    overlay: {
      light: 'rgba(26, 26, 46, 0.3)',
      medium: 'rgba(26, 26, 46, 0.5)',
      heavy: 'rgba(26, 26, 46, 0.85)',
    },
  },

  // Semantic colors  
  semantic: {
    success: '#00E676',
    successLight: '#4ADE80',
    error: '#FF1744',
    errorLight: '#FF5C5C',
    warning: '#FFCA28',
    warningLight: '#FBB040',
    info: '#2979FF',
    infoLight: '#60A5FA',
  },

  // Status colors
  online: '#00E676',
  offline: '#9C27B0',
  typing: '#FF4081',

  // Overlay colors (legacy compatibility)
  overlay: {
    light: 'rgba(26, 26, 46, 0.5)',
    dark: 'rgba(26, 26, 46, 0.85)',
  },

  // Glassmorphism
  glass: {
    background: 'rgba(102, 12, 108, 0.7)', // #660c6c glass
    border: 'rgba(225, 190, 231, 0.3)',    // Pale purple border
  },

  // Chat bubbles
  chat: {
    sent: '#FF4081',            // Pink
    received: '#2A0E35',        // Dark card color
    sentText: '#FFFFFF',
    receivedText: '#FFFFFF',
  },

  // Swipe actions (aliases for consistency)
  swipe: {
    like: '#00E676',
    nope: '#FF1744',
    superLike: '#2979FF',
  },

  // Gradients
  gradients: {
    primary: ['#FF4081', '#D500F9', '#7C3AED'] as const,
    secondary: ['#E1BEE7', '#4A148C'] as const,
    match: ['#FF4081', '#7C3AED', '#660c6c'] as const,
    dark: ['#660c6c', '#1a1a2e', '#660c6c'] as const,
    darkCard: ['transparent', 'rgba(42, 14, 53, 0.95)'] as const,
    message: {
      sent: ['#FF4081', '#D500F9'] as const,
      received: ['#2A0E35', '#1E1E2E'] as const,
    },
  },

  // Common
  white: '#FFFFFF',
  black: '#000000',
  transparent: 'transparent',
  error: '#FF1744',      // Alias for semantic.error
  success: '#00E676',    // Alias for semantic.success
  warning: '#FFCA28',    // Alias for semantic.warning
  info: '#2979FF',       // Alias for semantic.info
};

export default Colors;
