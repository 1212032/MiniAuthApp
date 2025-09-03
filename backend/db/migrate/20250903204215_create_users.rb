class CreateUsers < ActiveRecord::Migration[8.0]
  def change
    create_table :users, id: :uuid do |t|
      t.string :name
      t.date :date_birth
      t.string :email
      t.string :username
      t.string :password_digest

      t.timestamps
    end
  end
end
