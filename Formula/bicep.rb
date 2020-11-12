class Bicep < Formula
  desc "Bicep: next generation template language for Azure Resource Manager (ARM)"
  homepage "https://github.com/Azure/bicep"
  url "https://github.com/Azure/bicep/releases/download/v0.2.3/bicep-osx-x64"
  sha256 "a96e8aab745fe67dacc37d6d1f14dcf41fb439dabd8c40297fde030d2d4819a0"

  license "MIT"

  def install
    bin.install "bicep-osx-x64" => "bicep"
  end

  test do
  end
end
