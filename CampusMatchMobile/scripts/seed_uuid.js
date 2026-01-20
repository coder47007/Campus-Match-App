
const { createClient } = require('@supabase/supabase-js');
const { v4: uuidv4 } = require('uuid'); // Need uuid implementation? Or use crypto
const crypto = require('crypto');

const SUPABASE_URL = 'https://urtynghfmszehjwvligg.supabase.co';
const SUPABASE_ANON_KEY = 'eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJpc3MiOiJzdXBhYmFzZSIsInJlZiI6InVydHluZ2hmbXN6ZWhqd3ZsaWdnIiwicm9sZSI6ImFub24iLCJpYXQiOjE3Njg0MDk2MDEsImV4cCI6MjA4Mzk4NTYwMX0.nLQd8gmmRH76HaYZ4kkOTbwTnm8EiOtdJ6GF9aHuEuA';

const supabase = createClient(SUPABASE_URL, SUPABASE_ANON_KEY, {
    auth: { persistSession: false }
});

async function seed() {
    console.log('--- Seeding UUID Try ---');

    // Generate UUID
    const id = crypto.randomUUID();
    const email = `uuid_${Date.now()}@test.com`;

    const { data, error } = await supabase
        .from('Students')
        .insert({
            Id: id, // Try UUID
            Email: email,
            Name: 'UUID Test User',
            CreatedAt: new Date().toISOString(),
            PhoneNumber: '',
            InstagramHandle: ''
        })
        .select()
        .single();

    if (error) {
        console.error('Error:', error.message);
    } else {
        console.log('Success:', data);
    }
}

seed();
