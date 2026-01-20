import { useAuthStore } from '../../stores/authStore';

// Mock dependencies
jest.mock('../../services', () => ({
    authApi: {
        login: jest.fn(),
        logout: jest.fn().mockResolvedValue(undefined),
        register: jest.fn(),
    },
    profilesApi: {
        getMyProfile: jest.fn(),
    },
    loadStoredToken: jest.fn(),
    setAuthToken: jest.fn(),
}));

jest.mock('../../services/signalr', () => ({
    signalRService: {
        start: jest.fn(),
        stop: jest.fn(),
    },
}));

jest.mock('../../utils/storage', () => ({
    setStorageItem: jest.fn().mockResolvedValue(undefined),
    getStorageItem: jest.fn().mockResolvedValue(null),
    deleteStorageItem: jest.fn().mockResolvedValue(undefined),
}));

// Reset store before each test
beforeEach(() => {
    useAuthStore.setState({
        user: null,
        isLoading: false,
        isInitialized: false,
        token: null,
        error: null,
    });
});

describe('authStore', () => {
    it('should have initial state', () => {
        const state = useAuthStore.getState();

        expect(state.user).toBeNull();
        expect(state.isLoading).toBe(false);
        expect(state.token).toBeNull();
    });

    it('should update user via updateUser', () => {
        const mockUser = {
            id: 1,
            email: 'test@university.edu',
            name: 'Test User',
            age: 21,
            university: 'Test University',
        };

        useAuthStore.getState().updateUser(mockUser as any);

        expect(useAuthStore.getState().user).toEqual(mockUser);
    });

    it('should clear state on logout', async () => {
        // Set some state first
        useAuthStore.setState({
            user: { id: 1, name: 'Test' } as any,
            token: 'test-token',
        });

        // Logout
        await useAuthStore.getState().logout();

        // Verify cleared
        const state = useAuthStore.getState();
        expect(state.user).toBeNull();
        expect(state.token).toBeNull();
    });

    it('should clear error via clearError', () => {
        useAuthStore.setState({ error: 'Some error' });
        useAuthStore.getState().clearError();
        expect(useAuthStore.getState().error).toBeNull();
    });
});
